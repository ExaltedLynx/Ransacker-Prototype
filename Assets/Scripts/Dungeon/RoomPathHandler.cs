using System.Collections.Generic;
using UnityEngine;

public class RoomPathHandler : MonoBehaviour
{
    [SerializeField] private RoomObj thisRoom;
    [SerializeField] private Sprite pathArrow;
    [SerializeField] private Sprite pathStraight;
    [SerializeField] private Sprite pathTurn;
    [SerializeField] private Sprite pathTail;
    [SerializeField] private Sprite pathStart;

    LinkedListNode<Room> endOfPath => GameManager.Instance.playerSelectedPath.Last;
    RoomObj endOfPathObj => endOfPath.Value.roomObject;

    private void Start()
    {
        
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.pathSelectionTutEnabled)
            return;

        if (!GameManager.Instance.gameIsPaused && Input.GetMouseButton(0))
        {
            //Debug.Log("is dragging into room");
            //Only allow a room to be added to the path if its adjacent to the current end of the path
            if (endOfPath.Value == thisRoom.GetRoomData() || endOfPathObj.IsRoomAdjacent(thisRoom.GetRoomData()))
            {
                //Debug.Log("room is end of path or adjacent to end of path");
                //if the room the player dragged into is not the same as Last.Previous.Value, add it to the end of the linked list
                LinkedListNode<Room> secondToLastRoomObj = endOfPath.Previous;
                if ((GameManager.Instance.playerSelectedPath.Count == 1 || secondToLastRoomObj.Value != thisRoom.GetRoomData()) && thisRoom.pathRenderer.enabled == false && GameManager.Instance.playerSelectedPath.Count - 1 < GameManager.Instance.TotalMovement)
                {
                    //Debug.Log("adds adjacent room to path");
                    GameManager.Instance.playerSelectedPath.AddLast(thisRoom.GetRoomData());
                    thisRoom.pathRenderer.enabled = true;
                    
                }
                else if (GameManager.Instance.playerSelectedPath.Count > 1 && secondToLastRoomObj.Value == thisRoom.GetRoomData())
                {
                    //if that room is the same as Last.Previous.Value, remove the last node
                    endOfPathObj.pathRenderer.enabled = false;
                    GameManager.Instance.playerSelectedPath.RemoveLast();
                }
                HandlePathTexture();
                RoomPreviewController.Instance.UpdateStepsLeftText();
                if (endOfPath.Value == DungeonGenerator.Instance.currentFloor.bossRoom)
                    HandleStartGameButton.button.interactable = true;
                else
                    HandleStartGameButton.button.interactable = false;
            }
        }
    }

    public void SetStartRoomPathTexture()
    {
        endOfPathObj.pathRenderer.sprite = pathStart;
    }

    private void HandlePathTexture()
    {
        //DIRECTIONS ARE RELATIVE TO THE SECOND TO LAST ROOM IN THE PATH

        RoomObj endOfPath = GameManager.Instance.playerSelectedPath.Last.Value.roomObject;
        if (GameManager.Instance.playerSelectedPath.Count == 1)
        {
            //endOfPath.pathRenderer.enabled = true;
            endOfPath.pathRenderer.sprite = pathStart;
            return;
        }
            
        //Handles tail of the path
        var secondToLast = GameManager.Instance.playerSelectedPath.Last.Previous.Value.roomObject;
        Vector2 previousPathDirection = secondToLast.GetRoomData().roomPos - endOfPath.GetRoomData().roomPos;

        if (GameManager.Instance.playerSelectedPath.Count == 2)
        {
            secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            secondToLast.pathRenderer.sprite = pathTail;
            if (previousPathDirection == Vector2.left)
            {
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else if (previousPathDirection == Vector2.right)
            {
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else if (previousPathDirection == Vector2.up)
            {
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }

        //Handles head of the path
        endOfPath.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        endOfPath.pathRenderer.sprite = pathArrow;
        if (previousPathDirection == Vector2.left)
        {
            endOfPath.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (previousPathDirection == Vector2.right)
        {
            endOfPath.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (previousPathDirection == Vector2.up)
        {
            endOfPath.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }

        if (GameManager.Instance.playerSelectedPath.Count < 3)
            return;

        //handles vertical and horizontal movement
        var thirdToLast = GameManager.Instance.playerSelectedPath.Last.Previous.Previous;
        Vector2 fromDirection = thirdToLast.Value.roomPos - secondToLast.GetRoomData().roomPos;
        Vector2 toDirection = endOfPath.GetRoomData().roomPos - secondToLast.GetRoomData().roomPos;
        Vector2 sumDirection = fromDirection + toDirection;

        secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (sumDirection == Vector2.zero)
        {
            secondToLast.pathRenderer.sprite = pathStraight;
            if(fromDirection == Vector2.left || fromDirection == Vector2.right)
            {
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            return;
        }

        //handles turning
        secondToLast.pathRenderer.sprite = pathTurn;
        if (fromDirection == Vector2.left)
        {
            if (toDirection == Vector2.up)
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);
            else //down
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (fromDirection == Vector2.right) //prior room is to the right of secondToLast
        {
            //right->up is the default rotation
            if (toDirection == Vector2.down)
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (fromDirection == Vector2.down)
        {
            if (toDirection == Vector2.left)
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 180);
            else //right
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (fromDirection == Vector2.up)
        {
            //down->right is the default rotation
            if (toDirection == Vector2.left)
                secondToLast.pathRenderer.transform.localRotation = Quaternion.Euler(0, 0, 90);

        }
    }
}
