using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.Collisions;

namespace GamePlayground.Code.Entities
{
    public interface IEntity : IActorTarget
    {

        public void Update(GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
