﻿using System;
using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadConsole.Components
{
    /// <summary>
    /// Add to an <see cref="Entity"/> to sync the visibility and position offset with a parent <see cref="Console"/>. 
    /// </summary>
    public class EntityViewSyncComponent : UpdateComponent
    {
        private Point _oldPosition;
        private Rectangle _oldView;

        /// <summary>
        /// If set to true, controls the <see cref="ScreenObject.IsVisible"/> property of the attached object.
        /// </summary>
        public bool HandleIsVisible { get; set; } = true;

        /// <inheritdoc/>
        public override void OnAdded(ScreenObject host)
        {
            if (!(host is Entity))
                throw new Exception($"{nameof(EntityViewSyncComponent)} can only be added to an {nameof(Entity)}.");
        }


        /// <inheritdoc />
        public override void Update(ScreenObject hostObject)
        {
            var host = (Entity)hostObject;

            if (host.Parent is ScreenObjectSurface parent)
            {
                Rectangle parentViewPort = parent.Surface.GetViewRectangle();

                if (parentViewPort != _oldView || host.Position != _oldPosition)
                {
                    host.PositionOffset = new Point(-parentViewPort.Position.X, -parentViewPort.Position.Y).TranslateFont(parent.FontSize, host.FontSize);

                    if (HandleIsVisible)
                        host.IsVisible = parent.AbsoluteArea.Contains(host.AbsolutePosition);

                    _oldPosition = host.Position;
                    _oldView = parentViewPort;
                }
            }
        }

        /// <inheritdoc />
        public override void OnRemoved(ScreenObject host)
        {
            _oldPosition = (0, 0);
            _oldView = Rectangle.Empty;

            host.IsVisible = true;
            ((Entity)host).PositionOffset = (0, 0);
        }
    }
}
