using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// This class handles the dragging of cards. When a card is clicked it will follow the mouse and drag with it. 
/// </summary>
public class DragCard : MonoBehaviourPun
{
    private Vector3 offset;
    private Vector3 screenPoint;
    public Vector2 OriginalPosition;

    //This class is attached to all player cards. This can be cards owned by the player or by the shop
    //We use this delegate event to handle the clicking of a card in the shop 
    public delegate void _ShopCardClicked(PlayerCardDisplay cardClicked);

    public static event _ShopCardClicked ShopCardClicked;
    public static event Action <PlayerCardDisplay> CardDragged;
    public static event Action CardReleased; 

    private PlayerCardDisplay thisCard;

    private PhotonView RPCCardSelected;

    private void Awake()
    {
        //Set the original position of the card to its location in space to use a reference
        OriginalPosition = this.transform.position;
        
    }

    /// <summary>
    /// This method is ivoked when the mouse button is clicked
    /// </summary>
    public void OnMouseDown()
    {
        if (photonView.IsMine)
        {
            //used to grab the z coordinate of the game object 
            //We need to conver the position to world space so it works with nested objects
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            //Here we add the offset from the card and the mouse
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
                         new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            RPCCardSelected = this.GetComponent<PhotonView>();

            //if the card display has no hand container it means that the card is in the shop
            //We can use this to our advantage by adding shop functionality here
            
            //Debug.Log("Card is in Shop");
            PlayerCardDisplay cardClicked = this.gameObject.GetComponent<PlayerCardDisplay>();
            
            if (this.transform.parent.gameObject.GetComponent<HandContainer>() == null)
            {
                //THIS MIGHT BE BREAKING SHIT
                cardClicked.transform.position = OriginalPosition;
                
                ShopCardClicked?.Invoke(cardClicked);

                //photon view of our current card
                this.photonView.RPC("RPCOnMouseDownShop", RpcTarget.Others, RPCCardSelected.ViewID);
            }
            else
            {
                CardDragged?.Invoke(cardClicked);
                this.photonView.RPC("RPCOnMouseDown", RpcTarget.Others, RPCCardSelected.ViewID);
            }
        }
    }

    [PunRPC]
    private void RPCOnMouseDownShop(int cardID, Transform trans)
    {
        PhotonView foundCard = PhotonView.Find(cardID);
        if (foundCard)
        {
            PlayerCardDisplay cardClicked = foundCard.GetComponent<PlayerCardDisplay>();
            cardClicked.transform.position = OriginalPosition;
            ShopCardClicked?.Invoke(cardClicked);
        }
        else
        {
            Debug.Log("Photon View not found. CardID: " + cardID);
        }
    }

    [PunRPC]
    private void RPCOnMouseDown(int cardID)
    {
        PhotonView foundCard = PhotonView.Find(cardID);
        if (foundCard)
        {
            PlayerCardDisplay draggedCard = foundCard.GetComponent<PlayerCardDisplay>();
            CardDragged?.Invoke(draggedCard);
        }
        else
        {
            Debug.Log("Photon View not found. CardID: " + cardID);
        }
    }

    /// <summary>
    /// this method is called when the mouse button is released
    /// </summary>
    public void OnMouseUp()
    {
        if (photonView.IsMine)
        {
            CardReleased?.Invoke();
            //if there is a gameobject and the card is not in the play zone we will return the card to the original position
            if (this.gameObject != null && PlayZone.cardInPlayZone == false)
            {
                this.transform.position = OriginalPosition;
            }

            //photon view of our current card
            RPCCardSelected = this.GetComponent<PhotonView>();
            this.photonView.RPC("RPCOnMouseUp", RpcTarget.Others, RPCCardSelected.ViewID);
        }
    }

    [PunRPC]
    private void RPCOnMouseUp(int cardID)
    {
        PhotonView foundCard = PhotonView.Find(cardID);
        if (foundCard)
        {
            
            transform.position = OriginalPosition;
            CardReleased?.Invoke();
        }
        else
        {
            Debug.Log("Photon View not found. CardID: " + cardID);
        }
    }

    /// <summary>
    /// This method is called when the card is being dragged
    /// </summary>
    public void OnMouseDrag()
    {
        if (photonView.IsMine)
        {
            //if the card display has no hand container it means that the card is in the shop
            //if that is the case we do not want to drag the card. 
            if (this.transform.parent.gameObject.GetComponent<HandContainer>() == null)
            {
                return;
            }

            //Debug.Log("Attempting to drag and the object is draggable");

            //stores position of cursor in screen space
            Vector2 cursorScreenPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //grabs the position of the mouse cursor and converts to world space
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;

            //updates position of game object
            transform.position = cursorPosition;

            //current card
            thisCard = this.gameObject.GetComponent<PlayerCardDisplay>();

            //photon view of our current card
            RPCCardSelected = this.GetComponent<PhotonView>();

            //RPC call the current card photon ID and the changed position
            this.photonView.RPC("RPCOnMouseDrag", RpcTarget.Others, RPCCardSelected.ViewID, transform);
        }
    }

    [PunRPC]
    private void RPCOnMouseDrag(int cardID, Transform trans)
    {
        //find the phototon view associted with the given ID
        PhotonView foundCard = PhotonView.Find(cardID);
        if (foundCard)
        {
            //update the position of the card
            foundCard.transform.position = trans.position;
        }
        else
        {
            Debug.Log("Photon View not found. CardID: " + cardID);
        }
    }
}