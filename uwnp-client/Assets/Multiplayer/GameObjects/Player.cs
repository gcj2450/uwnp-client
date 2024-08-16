using System;
using UnityEngine;

namespace MultiplayerProject.Source
{
    public class Player : INetworkedObject
    {
        public bool Active;
        public int Health;

        public int Width { get; set; }
        public int Height { get; set; }

        public Vector2 Position { get { return PlayerState.Position; } }
        public float Rotation { get { return PlayerState.Rotation; } }

        public string NetworkID { get; set; }
        public int LastSequenceNumberProcessed { get; set; }
        public KeyboardMovementInput LastKeyboardMovementInput { get; set; }

        public struct ObjectState
        {
            public Vector2 Position; // VECTOR2 NOT SERIALISABLE
            public Vector2 Velocity;
            public float Rotation;
            public float Speed;
        }

        // Animation representing the player
        private GameObject PlayerAnimation;

        // This is the player state that is drawn onto the screen. It is gradually
        // interpolated from the previousState toward the simultationState, in
        // order to smooth out any sudden jumps caused by discontinuities when
        // a network packet suddenly modifies the simultationState.
        protected ObjectState PlayerState;

        public Player()
        {
            PlayerState.Position = Vector2.zero;
            PlayerState.Velocity = Vector2.zero;
            PlayerState.Rotation = 0;

            Width = 115; // HARDCODED WIDTH AND HEIGHT
            Height = 69;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
        }

        public void Initialize()
        {
            // Load the player resources
            GameObject playerAnimation = GameObject.CreatePrimitive(PrimitiveType.Cube);

            PlayerAnimation = playerAnimation;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            if (PlayerAnimation != null)
            {
                PlayerAnimation.transform.position= PlayerState.Position;
                Vector3 angel = PlayerAnimation.transform.localEulerAngles;
                angel.y= PlayerState.Rotation;
                PlayerAnimation.transform.localEulerAngles = angel;

                //PlayerAnimation.Update(gameTime);
            }
        }

        public void Update(float deltaTime)
        {
            Update(ref PlayerState, deltaTime);
        }

        public void Update(ref ObjectState state, float deltaTime)
        {
            // Limit the max speed
            if (state.Speed > 15f)
                state.Speed = 15f;
            else if (state.Speed < -15f)
                state.Speed = -15f;

            Vector2 direction = new Vector2((float)Math.Cos(state.Rotation),
                        (float)Math.Sin(state.Rotation));
            direction.Normalize();

            state.Position += direction * state.Speed;
            state.Speed *= 0.95f;

            // Make sure that the player does not go out of bounds
            state.Position.x = Mathf.Clamp(state.Position.x, 0, 480);
            state.Position.y = Mathf.Clamp(state.Position.y, 0, 800);
        }

        public void SetPlayerState(PlayerUpdatePacket packet)
        {
            PlayerState.Position = new Vector2(packet.XPosition, packet.YPosition);
            PlayerState.Rotation = packet.Rotation;
            PlayerState.Speed = packet.Speed;
            // VELOCITY????/
        }

        public void ApplyInputToPlayer(KeyboardMovementInput input, float deltaTime)
        {
            ApplyInputToPlayer(ref PlayerState, input, deltaTime);
        }

        public void ApplyInputToPlayer(ref ObjectState state, KeyboardMovementInput input, float deltaTime)
        {
            if (input.DownPressed)
            {
                state.Speed -= 12f * deltaTime;
            }

            if (input.UpPressed)
            {
                state.Speed += 12f * deltaTime;
            }

            if (input.LeftPressed)
            {
                state.Rotation -= 2f * deltaTime;
            }

            if (input.RightPressed)
            {
                state.Rotation += 2f * deltaTime;
            }
        }

        public PlayerUpdatePacket BuildUpdatePacket()
        {
            Vector2 pos = new Vector2((float)Math.Round((decimal)PlayerState.Position.x, 1), (float)Math.Round((decimal)PlayerState.Position.y, 1));
            float speed = (float)Math.Round((decimal)PlayerState.Speed, 1);
            float rot = (float)Math.Round((decimal)PlayerState.Rotation, 1);
            return NetworkPacketFactory.Instance.MakePlayerUpdatePacket(pos.x, pos.y, speed, rot);
        }
    }
}