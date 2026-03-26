using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;

namespace DungeonSlime
{
    public class Game1 : Core
    {
        // Defines the slime animated sprite.
        private AnimatedSprite _slime;

        // Defines the bat animated sprite.
        private AnimatedSprite _bat;

        // Tracks the position of the slime.
        private Vector2 _slimePosition;

        // Speed multiplier when moving.
        private const float MOVEMENT_SPEED = 5.0f;

        // Use a queue directly for input buffering
        private Queue<Vector2> _inputBuffer;
        private const int MAX_BUFFER_SIZE = 2;

        public Game1() : base("Dungeon Slime", 1280, 720, false)
        {

        }

        protected override void Initialize()
        {
            _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create the texture atlas from the XML configuration file
            TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

            // Create the slime animated sprite from the atlas.
            _slime = atlas.CreateAnimatedSprite("slime-animation");
            _slime.Scale = new Vector2(4.0f, 4.0f);

            // Create the bat animated sprite from the atlas.
            _bat = atlas.CreateAnimatedSprite("bat-animation");
            _bat.Scale = new Vector2(4.0f, 4.0f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the slime animated sprite.
            _slime.Update(gameTime);

            // Update the bat animated sprite.
            _bat.Update(gameTime);

            // Check for keyboard input and handle it.
            CheckKeyboardInput();

            // Check for gamepad input and handle it.
            CheckGamePadInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumPurple);

            // Begin the sprite batch to prepare for rendering.
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw the slime sprite.
            _slime.Draw(SpriteBatch, _slimePosition);

            // Draw the bat sprite 10px to the right of the slime.
            _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));

            // Always end the sprite batch when finished.
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        #region input handling
        private void CheckKeyboardInput()
        {
            // Get the state of keyboard input
            KeyboardState keyboard = Keyboard.GetState();
            Vector2 newDirection = Vector2.Zero;

            // If the space key is held down, the movement speed increases by 1.5
            float speed = MOVEMENT_SPEED;
            if (keyboard.IsKeyDown(Keys.Space))
            {
                speed *= 1.5f;
            }

            // Input buffer checks for directional keys and adds them to the buffer if they are pressed.
            if (keyboard.IsKeyDown(Keys.Up))
            {
                newDirection = -Vector2.UnitY;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                newDirection = Vector2.UnitY;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                newDirection = -Vector2.UnitX;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                newDirection = Vector2.UnitX;
            }

            // Only add if a valid direction and does not exceed the buffer size.
            if (newDirection != Vector2.Zero && _inputBuffer.Count < MAX_BUFFER_SIZE)
            {
                _inputBuffer.Enqueue(newDirection);
            }

            if (_inputBuffer.Count > 0)
            {
                Vector2 nextDirection = _inputBuffer.Dequeue();
                _slimePosition += nextDirection * speed;
            }
        }

        private void CheckGamePadInput()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            // If the A button is held down, the movement speed increases by 1.5
            // and the gamepad vibrates as feedback to the player.
            float speed = MOVEMENT_SPEED;
            if (gamePadState.IsButtonDown(Buttons.A))
            {
                speed *= 1.5f;
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
            }

            // Check thumbstick first since it has priority over which gamepad input
            // is movement.  It has priority since the thumbstick values provide a
            // more granular analog value that can be used for movement.
            if (gamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                _slimePosition.X += gamePadState.ThumbSticks.Left.X * speed;
                _slimePosition.Y -= gamePadState.ThumbSticks.Left.Y * speed;
            }
            else
            {
                // If DPadUp is down, move the slime up on the screen.
                if (gamePadState.IsButtonDown(Buttons.DPadUp))
                {
                    _slimePosition.Y -= speed;
                }

                // If DPadDown is down, move the slime down on the screen.
                if (gamePadState.IsButtonDown(Buttons.DPadDown))
                {
                    _slimePosition.Y += speed;
                }

                // If DPapLeft is down, move the slime left on the screen.
                if (gamePadState.IsButtonDown(Buttons.DPadLeft))
                {
                    _slimePosition.X -= speed;
                }

                // If DPadRight is down, move the slime right on the screen.
                if (gamePadState.IsButtonDown(Buttons.DPadRight))
                {
                    _slimePosition.X += speed;
                }
            }
        }

        #endregion input handling
    }
}
