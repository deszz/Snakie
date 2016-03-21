using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snakie.Scenes;
using Snakie.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie.World
{
    public class Snake : GameObject
    {
        private const int segmentSideSize = 32;
        private const int segmentGap = 3;
        private const int segmentMultiplier = 5;

        private const int minSegmentsCount = 3;
        private const int maxSegmentsCount = int.MaxValue;

        public event EventHandler Death;
        public event EventHandler SelfCollision;
        public event EventHandler BoundsCollision;

        public float BonusSpeed
        { get; set; }
        public float BonusMovement
        { get; set; }

        public bool IsPaused
        { get; set; }

        private List<Vector2> segments;
        private Point segmentSize;

        private Texture2D segmentTexture;
        private Texture2D headTexture;
        private Texture2D holeTexture;

        private float direction; // angle in degrees
        private float speed;
        private float movement;

        private float timeSinceStart;

        private Vector2 holePosition;
        private float holeAlpha;

        public Snake(int initialSegments = 3)
        {
            segments = new List<Vector2>();
            segmentSize = new Point(segmentSideSize, segmentSideSize);

            segmentTexture = App.LoadRes<Texture2D>("World/Snake/Segment");
            headTexture = App.LoadRes<Texture2D>("World/Snake/Head");
            holeTexture = App.LoadRes<Texture2D>("World/Hole");

            direction = 90;
            speed = 300;
            movement = 2f;

            holeAlpha = 1f;

            PlaceSegments(initialSegments, out holePosition);
        }

        public bool IsHeadIntersects(Rectangle rect)
        {
            return GetHeadSegment().Intersects(rect);
        }

        public void AddSegment()
        {
            for (int i = 0; i < segmentMultiplier; ++i)
                segments.Add(segments.Last());
        }

        public void RemoveSegment()
        {
            int toRemove = Math.Min(segmentMultiplier,
                                    segments.Count - (segmentMultiplier * minSegmentsCount));

            for (int i = 0; i < toRemove; ++i)
                segments.RemoveAt(segments.Count - i - 1);
        }

        #region Update

        public override void OnUpdate()
        {
            if (!IsPaused)
            {
                var deltaTime = App.FrameTime;
                timeSinceStart += deltaTime;

                CheckSelfCollisions();
                CheckBoundsCollisions();

                UpdateController(deltaTime);
                UpdateMovement(deltaTime);
            }

            base.OnUpdate();
        }

        private void UpdateController(float deltaTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
                direction += deltaTime * (speed * 0.5f) * movement + BonusMovement;
            if (state.IsKeyDown(Keys.Right))
                direction -= deltaTime * (speed * 0.5f) * movement + BonusMovement;
        }

        private void UpdateMovement(float deltaTime)
        {
            var deltaX = (float)Math.Cos(Utils.DegToRad(direction)) * deltaTime * (speed + BonusSpeed);
            var deltaY = (float)-Math.Sin(Utils.DegToRad(direction)) * deltaTime * (speed + BonusSpeed);

            segments[0] = Vector2.Add(segments[0], new Vector2(deltaX, deltaY));

            for (int i = segments.Count - 1; i > 0; --i)
                UpdateSegmentMovement(i);
        }

        private void UpdateSegmentMovement(int index)
        {
            var segmentsDiff = segments[index] - segments[index - 1];
            var segmentsDist = segmentsDiff.Length();

            if (segmentsDist > segmentGap)
            {
                var distToMove = segmentsDist - segmentGap;
                var translation = segmentsDiff.Normalized() * distToMove;

                segments[index] -= translation;
            }
        }

        #endregion

        #region Draw

        public override void OnDraw(SpriteBatch sBatch)
        {
            DrawHole(sBatch);
            DrawTailSegments(sBatch);
            DrawHead(sBatch);

            base.OnDraw(sBatch);
        }

        private void DrawHole(SpriteBatch sBatch)
        {
            if (holeAlpha > 0)
            {
                var holeRect = new Rectangle(holePosition.ToPoint(), segmentSize);
                sBatch.Draw(holeTexture, holeRect, new Color(Color.Black, holeAlpha));
            }

            if (segments.Last() != holePosition)
            {
                holeAlpha -= App.FrameTime;
            }
        }

        private void DrawTailSegments(SpriteBatch sBatch)
        {
            for (int i = 1; i < segments.Count; ++i)
            {
                // App.__Debug_DrawRect(sBatch, GetSegmentRect(i));
                sBatch.Draw(segmentTexture, GetSegmentRect(i), Color.Black);
            }
        }

        private void DrawHead(SpriteBatch sBatch)
        {
            var rect = GetSegmentRect(0);
            rect.Location += new Point(segmentSideSize / 2);

            var rotation = (float)-Utils.DegToRad(direction);
            var origin = new Vector2(headTexture.Width / 2f, headTexture.Height / 2f);

            sBatch.Draw(
                headTexture,
                destinationRectangle: rect,
                rotation: rotation,
                origin: origin,
                color: Color.White);
        }

        #endregion

        private void PlaceSegments(int segmentsCount, out Vector2 holePosition)
        {
            if (segments.Count > 0)
                segments.Clear();

            var screenCenter = App.Viewport.GetScreenCenter();
            holePosition = new Vector2(screenCenter.X - (segmentSideSize / 2),
                                       screenCenter.Y - (segmentSideSize / 2));

            int _segmentsCount = Math.Max(minSegmentsCount, segmentsCount * segmentMultiplier);
            for (int i = 0; i < _segmentsCount; i++)
                segments.Add(holePosition);
        }

        private void CheckSelfCollisions()
        {
            if (timeSinceStart < 2)
                return;

            var head = GetHeadSegment();

            for (int i = minSegmentsCount * segmentMultiplier; i < segments.Count; ++i)
            {
                if (head.Intersects(GetSegmentRect(i)))
                {
                    OnSelfCollision(i);
                    return;
                }
            }
        }

        private void CheckBoundsCollisions()
        {
            var head = GetSegmentRect(0);

            if (head.X < 0 || (head.X + segmentSize.X) > App.Viewport.Width ||
                head.Y < 0 || (head.Y + segmentSize.Y) > App.Viewport.Height)
                OnBoundsCollision();
        }

        private void OnSelfCollision(int segmentIndex)
        {
            SelfCollision?.Invoke(this, EventArgs.Empty);
            OnDeath();
        }

        private void OnBoundsCollision()
        {
            BoundsCollision?.Invoke(this, EventArgs.Empty);
            OnDeath();
        }

        private void OnDeath()
        {
            Death?.Invoke(this, EventArgs.Empty);
        }

        #region Utils

        private Rectangle GetHeadSegment()
        {
            var head = GetSegmentRect(0);

            head.X += head.Width / 8;
            head.Y += head.Height / 8;
            head.Width -= head.Width / 4;
            head.Height -= head.Height / 4;

            return head;
        }

        private Rectangle GetSegmentRect(int index)
        {
            return new Rectangle(segments[index].ToPoint(), segmentSize);
        }

        #endregion
    }
}
