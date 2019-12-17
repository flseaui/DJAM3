using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using static UserInput.InputType;

public class UserInput : MonoBehaviour
{

    public int PlayerId;

    private Player _player;

    public InputBox Inputs = new InputBox();
    
    void Awake()
    {
        _player = ReInput.players.GetPlayer(PlayerId);
        Inputs.Inputs = new bool[3];
    }

    // Update is called once per frame
    void Update()
    {
        Inputs[MoveLeft] = _player.GetButton("MoveLeft");
        Inputs[MoveRight] = _player.GetButton("MoveRight");
        Inputs[Jump] = _player.GetButtonDown("Jump");
    }

    public enum InputType
    {
        MoveLeft,
        MoveRight,
        Jump
    }    

    public class InputBox
    {
        public bool[] Inputs;

        public bool this[InputType key]
        {
            get => Inputs[(int) key];
            set => Inputs[(int) key] = value;
        }
    }
}