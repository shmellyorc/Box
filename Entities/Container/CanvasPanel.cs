namespace Box.Entities.Container
{
    /// <summary>
    /// Represents a special type of panel that follows the camera. 
    /// </summary>
    /// <remarks>
    /// This panel is designed to keep its contents within the view of the camera, 
    /// ensuring that UI elements remain visible as the camera moves.
    /// </remarks>
    public class CanvasPanel : Panel
    {
        private Vect2 _offset;

        /// <summary>
        /// Gets or sets the offset of the panel. 
        /// </summary>
        /// <remarks>
        /// This offset determines the position of the panel relative to the camera's view.
        /// </remarks>
        public Vect2 Offset
        {
            get => _offset;
            set
            {
                var oldValue = _offset;
                _offset = value;

                if (_offset != oldValue)
                {
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasPanel"/> class with the specified children entities.
        /// </summary>
        /// <param name="children">An array of child entities to be added to the panel.</param>
        public CanvasPanel(params Entity[] children)
            : base(children) { }

        /// <summary>
        /// Updates the state of the <see cref="CanvasPanel"/>. 
        /// This method is called once per frame and is responsible for updating the panel's position 
        /// and its contents to follow the camera.
        /// </summary>
        protected override void Update()
        {
            if (Camera == null)
                return;

            Position = (Camera.Position - (BE.Renderer.Center)) + Offset;

            base.Update();
        }
    }
}
