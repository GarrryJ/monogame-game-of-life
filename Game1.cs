using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameOfLife
{
    public class Pair<T, U> 
    {
        public Pair() {
        }

        public Pair(T first, U second) {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
    public class GOLEngine : Game
    {
        private const int SPEED = 3;
        private Queue<Pair<int, int>> queue;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D rectangleBlock;
        private bool paused;
        private int speed;
        private bool[,] array;
        private MouseState currentMouseState;
        private KeyboardState currentKeyboardState;

        public GOLEngine()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            queue = new Queue<Pair<int, int>>();
            speed = SPEED;
            Window.Title = "Game Of Life";
            _graphics.PreferredBackBufferHeight = 1080;
			_graphics.PreferredBackBufferWidth = 1920;
            _graphics.IsFullScreen = true;
			_graphics.ApplyChanges();
            paused = true;
            array = new bool[192, 108];
            for (int i = 0; i < 192; i++)
                for (int j = 0; j < 108; j++)
                    array[i, j] = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            rectangleBlock = new Texture2D(GraphicsDevice, 1, 1);
            Color xnaColorBorder = new Color(255, 255, 255);
            rectangleBlock.SetData(new[] { xnaColorBorder });
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private void invertCell(int x, int y)
        {
            if (array[x, y])
                array[x, y] = false;
            else
                array[x, y] = true;
        }
        private int inc(int n, bool x)
        {
            int max = 108;
            if (x)
                max = 192;
            if (n == max - 1)
                return 0;
            else 
                return n + 1;
        }

        private int dec(int n, bool x)
        {
            int max = 108;
            if (x)
                max = 192;
            if (n == 0)
                return max - 1;
            else 
                return n - 1;
        }

        private int neighbors(int x, int y)
        {
            int result = 0;

            x = dec(x, true);
            y = dec(y, false);

            if (array[x, y])
                result++;

            x = inc(x, true);
            if (array[x, y])
                result++;

            x = inc(x, true);
            if (array[x, y])
                result++;
            
            y = inc(y, false);
            if (array[x, y])
                result++;

            y = inc(y, false);
            if (array[x, y])
                result++;

            x = dec(x, true);
            if (array[x, y])
                result++;

            x = dec(x, true);
            if (array[x, y])
                result++;

            y = dec(y, false);
            if (array[x, y])
                result++;


            return result;
        }

        private void refreshBoard()
        {
            for (int x = 0; x < 192; x++)
                for (int y = 0; y < 108; y++)
                {
                    int n = neighbors(x, y);
                    if ((!array[x, y]) && (n == 3))
                        queue.Enqueue(new Pair<int, int>(x, y));
                    else if ((array[x, y]) && ((n < 2) || (n > 3)))
                        queue.Enqueue(new Pair<int, int>(x, y));
                }
            while (!queue.Count.Equals(0))
            {
                Pair<int, int> coor = queue.Dequeue();
                invertCell(coor.First, coor.Second);
            }
        }

        private void clear()
        {
            for (int i = 0; i < 192; i++)
                for (int j = 0; j < 108; j++)
                    array[i, j] = false;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            MouseState lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (lastKeyboardState.IsKeyDown(Keys.S) && currentKeyboardState.IsKeyUp(Keys.S))
            {
                if (paused)
                    paused = false;
                else
                    paused = true;
            }

            if (lastKeyboardState.IsKeyDown(Keys.C) && currentKeyboardState.IsKeyUp(Keys.C))
            {
                clear();
                paused = true;
            }
                
            
            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                invertCell(currentMouseState.X / 10, currentMouseState.Y / 10);
            }

            if (!paused)
            {
                speed--;
                if (speed == 0)
                {
                    speed = SPEED;
                    refreshBoard();
                }
            }
                

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);
            _spriteBatch.Begin();

            for (int i = 0; i < 192; i++)
                for (int j = 0; j < 108; j++)
                    if (array[i, j])
                        _spriteBatch.Draw(rectangleBlock, new Rectangle(i * 10, j * 10, 10, 10), new Color(255, 255, 0)); 

            if (paused)
            {
            for (int i = 1; i < 192; i++)
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(i * 10, 0, 1, 1080), new Color(100, 100, 0)); 
                
                for (int j = 1; j < 108; j++)
                    _spriteBatch.Draw(rectangleBlock, new Rectangle(0, j * 10, 1920, 1), new Color(100, 100, 0)); 
            }
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
