using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelSelectPlayer : MonoBehaviour {
    [SerializeField] private LSManager lsManager;
    [SerializeField] private MapPoint currentPoint;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float moveSpeed = 10f;

    private MapPoint _selectedPoint;
    private bool _levelLoading;
    private bool _isSelectPoint;

    public MapPoint CurrentPoint {
        set => currentPoint = value;
    }

    void Update() {
        if (_isSelectPoint) MovePlayer();
        ChangePosition();
    }

    private void ChangePosition() {
        var oldPosition = transform.position;
        transform.position = Vector3.MoveTowards(
            oldPosition,
            currentPoint.transform.position,
            moveSpeed * Time.deltaTime
        );
    }

    private bool IsLevelNotLocked() {
        return !currentPoint.IsLocked;
    }

    private bool IsLevelExists() {
        return currentPoint.LevelToLoad != null;
    }

    private bool IsLevel() {
        return currentPoint.IsLevel;
    }

    public void OnMovement(InputAction.CallbackContext context) {
        var input = context.ReadValue<Vector2>();
        var inputX = input.x;
        var inputY = input.y;
        if (CanMove()) {
            if (inputX > .5f) {
                MoveToRight();
            } else if (inputX < -.5f) {
                MoveToLeft();
            } else if (inputY > .5f) {
                MoveToUp();
            } else if (inputY < -.5f) {
                MoveToDown();
            }
        }
    }

    private Boolean CanMove() {
        return Vector3.Distance(transform.position, currentPoint.transform.position) < .1f && !_levelLoading;
    }

    public void OnPlay(InputAction.CallbackContext context) {
        if (CanMove() && IsLevel() && IsLevelExists() && IsLevelNotLocked()) {
            _levelLoading = true;
            lsManager.LoadLevel(currentPoint.LevelToLoad);
        }
    }

    public void OnTouch(InputAction.CallbackContext context) {
    }

    public void OnMouseClick(InputAction.CallbackContext context) {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit) {
            _selectedPoint = hit.transform.gameObject.GetComponent<MapPoint>();
            _isSelectPoint = true;
        }
    }

    private void MovePlayer() {
        Debug.Log("start moving");
        if (_selectedPoint == currentPoint) {
            _isSelectPoint = false;
            Debug.Log("end moving");
            return;
        }
        if (CanMove()) SetNextPoint(_selectedPoint.ID > currentPoint.ID ? currentPoint.Next : currentPoint.Prev);
    }


    private void MoveToRight() {
        if (currentPoint.Right != null) {
            SetNextPoint(currentPoint.Right);
        }
    }

    private void MoveToLeft() {
        if (currentPoint.Left != null) {
            SetNextPoint(currentPoint.Left);
        }
    }

    private void MoveToUp() {
        if (currentPoint.Up != null) {
            SetNextPoint(currentPoint.Up);
        }
    }

    private void MoveToDown() {
        if (currentPoint.Down != null) {
            SetNextPoint(currentPoint.Down);
        }
    }

    private void SetNextPoint(MapPoint next) {
        currentPoint = next;
        audioManager.PlaySFX(5);
    }
}