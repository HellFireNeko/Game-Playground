using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace GamePlayground.Code.Entities
{
    public class Player : IEntity
    {
        private readonly Game1 Game;
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }

        public RectangleF BoundingBox { get => new(Position, Size); }

        public float Speed = 50;

        public Player(Game1 game)
        {
            Game = game;
        }

        public void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.PenetrationVector.Y > 0 && Velocity.Y > 0)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }
            Position -= collisionInfo.PenetrationVector;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity * gameTime.GetElapsedSeconds() * Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(BoundingBox, Color.Red);
        }
    }
}
