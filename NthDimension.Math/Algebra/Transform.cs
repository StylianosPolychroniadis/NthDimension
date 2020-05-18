namespace NthDimension.Algebra
{
    
    /// <summary>
    /// The Transform class represents a 3D SRT (scaling/rotation/translation) matrix. 
    /// Generally this class is used to represent a world matrix and has functions to conviently set
    /// each component, compute the SRT matrix, and parent-child transform combination.
    /// </summary>
    public sealed class Transform // TODO::Serialize
    {

        private Vector3 _scale;
        private Quaternion _rotation;
        private Vector3 _translation;
        private Matrix4 _cachedMatrix;
        private bool _cacheRefresh;

        /// <summary>
        /// Gets or sets the scaling vector.
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets or sets the rotation quaternion.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets or sets the translation vector.
        /// </summary>
        public Vector3 Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                _cacheRefresh = true;
            }
        }

        /// <summary>
        /// Gets the computed SRT matrix.
        /// </summary>
        public Matrix4 Matrix
        {
            get
            {
                if (_cacheRefresh)
                {
                    Matrix4 scaleM;
                    Matrix4 rotationM;
                    Matrix4 translationM;

                    scaleM          = Matrix4.CreateScale(_scale);                      // Matrix4.FromScale(ref _scale, out scaleM);
                    rotationM       = Matrix4.CreateFromQuaternion(_rotation);          // Matrix4.FromQuaternion(ref _rotation, out rotationM);
                    translationM    = Matrix4.CreateTranslation(_translation);          // Matrix4.FromTranslation(ref _translation, out translationM);

                    _cachedMatrix = scaleM * rotationM * translationM;
                    _cacheRefresh = false;
                }

                return _cachedMatrix;
            }
        }

        /// <summary>
        /// Creates a new instance of a Transform with unit scaling, no translation, and an identity rotation quaternion.
        /// </summary>
        public Transform()
        {
            _scale = Vector3.One;
            _rotation = Quaternion.Identity;
            _translation = Vector3.Zero;
            _cachedMatrix = Matrix4.Identity;
            _cacheRefresh = false;
        }

        /// <summary>
        /// Creates a new instance of a Transform from the components of the supplied prototype.
        /// </summary>
        /// <param name="prototype">Transform to copy from</param>
        public Transform(Transform prototype)
        {
            _scale = prototype._scale;
            _rotation = prototype._rotation;
            _translation = prototype._translation;
        }

        /// <summary>
        /// Creates a new instance of a Transform with the supplied components.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        public Transform(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            _scale = scale;
            _rotation = rotation;
            _translation = translation;
        }

        /// <summary>
        /// Sets the transform with the store from the supplied transform.
        /// </summary>
        /// <param name="transform">Transform to copy from</param>
        public void Set(Transform transform)
        {
            _scale = transform._scale;
            _rotation = transform._rotation;
            _translation = transform._translation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform from a (S)cale-(R)otation-(T)ranslation matrix.
        /// </summary>
        /// <param name="matrix">Matrix4 to decompose the scale/rotation/translation components from.</param>
        public void Set(Matrix4 matrix)
        {
            matrix.Decompose(out _scale, out _rotation, out _translation);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform with the supplied components.
        /// </summary>
        /// <param name="scale">Scaling vector</param>
        /// <param name="rotation">Rotation quaternion</param>
        /// <param name="translation">Translation vector</param>
        public void Set(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            _scale = scale;
            _rotation = rotation;
            _translation = translation;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's translation vector from the supplied coordinates.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        public void SetTranslation(float x, float y, float z)
        {
            _translation.X = x;
            _translation.Y = y;
            _translation.Z = z;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's scaling vector with the supplied scaling factors for each axis.
        /// </summary>
        /// <param name="x">Scaling on x axis</param>
        /// <param name="y">Scaling on y axis</param>
        /// <param name="z">Scaling on z axis</param>
        public void SetScale(float x, float y, float z)
        {
            _scale.X = x;
            _scale.Y = y;
            _scale.Z = z;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform's scaling vector with a single uniform index.
        /// </summary>
        /// <param name="scale">Uniform scaling index</param>
        public void SetScale(float scale)
        {
            _scale.X = scale;
            _scale.Y = scale;
            _scale.Z = scale;
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the rotation quaternion from a rotation matrix. The matrix *must* represent a rotation!
        /// This method does NOT check if if the matrix is a valid rotation matrix.
        /// </summary>
        /// <param name="rotMatrix"></param>
        public void SetRotation(Matrix3 rotMatrix)
        {
            _rotation = Quaternion.FromMatrix(rotMatrix);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Sets the transform to its identity state - scaling vector is all 1's, translation is all 0's, and
        /// the rotation quaternion is the identity.
        /// </summary>
        public void SetIdentity()
        {
            _scale = Vector3.One;
            _rotation = Quaternion.Identity;
            _translation = Vector3.Zero;
            _cachedMatrix = Matrix4.Identity;
            _cacheRefresh = false;
        }

        /// <summary>
        /// Returns the calculated row vector of the 3x3 rotation matrix (represented by the quaternion).
        /// Row 0 is Right, row 1 is Up, and row 2 is Forward.
        /// </summary>
        /// <param name="i">Row index, must be between 0 and 2</param>
        /// <returns>Column vector of the rotation matrix</returns>
        /// <exception cref="Tesla.Core.TeslaException">Throws an exception if index is not in range</exception>
        public Vector3 GetRotationVector(int i)
        {
            if (i > 2 || i < 0)
            {
                throw new System.Exception("GetRotationColumn index must be between 0 and 2.");
            }
            Vector3 column;
            Quaternion.GetRotationVector(ref _rotation, i, out column);
            return column;
        }

        /// <summary>
        /// Combines this transform with a transform that represents its "parent". This is a convience method
        /// used by the engine's scene graph primarily.
        /// </summary>
        /// <param name="parent">Parent transform</param>
        public void CombineWithParent(Transform parent)
        {
            //Multiply scaling
            Vector3.Multiply(ref parent._scale, ref _scale, out _scale);

            //Multiply rotation
            Quaternion.Multiply(ref parent._rotation, ref _rotation, out _rotation);

            //Combine translation
            Vector3.Multiply(ref _translation, ref parent._scale, out _translation);
            Vector3.Transform(ref _translation, ref parent._rotation, out _translation);
            Vector3.Add(ref _translation, ref parent._translation, out _translation);
            _cacheRefresh = true;
        }

        /// <summary>
        /// Interpolates between two transforms, setting the result to this transform. Slerp is applied
        /// to the rotations and Lerp to the translation/scale.
        /// </summary>
        /// <param name="start">Starting transform</param>
        /// <param name="end">Ending transform</param>
        /// <param name="percent">Percent to interpolate between the two transforms, must be between 0 and 1</param>
        public void InterpolateTransforms(Transform start, Transform end, float percent)
        {
            _rotation = Quaternion.Slerp(start._rotation, end._rotation, percent);
            Vector3.Lerp(ref start._scale, ref end._scale, percent, out _scale);
            Vector3.Lerp(ref start._translation, ref end._translation, percent, out _translation);
        }

        /// <summary>
        /// Transforms the supplied vector.
        /// </summary>
        /// <param name="v">Vector3 to be transformed</param>
        /// <returns>Transformed vector</returns>
        public Vector3 TransformVector(Vector3 v)
        {
            Vector3 result;
            Vector3.Transform(ref v, ref _rotation, out result);
            Vector3.Multiply(ref result, ref _scale, out result);
            Vector3.Add(ref result, ref _translation, out result);
            return result;
        }

        /// <summary>
        /// Transforms the supplied vector.
        /// </summary>
        /// <param name="v">Vector3 to be transformed</param>
        /// <param name="result">Existing Vector3 to hold result</param>
        public void TransformVector(ref Vector3 v, out Vector3 result)
        {
            Vector3.Transform(ref v, ref _rotation, out result);
            Vector3.Multiply(ref result, ref _scale, out result);
            Vector3.Add(ref result, ref _translation, out result);
        }

        /// <summary>
        /// Serializes the transform.
        /// </summary>
        /// <param name="output">Output to write to</param>
        public void Write(/*ISavableWriter output*/)
        {
            //output.Write("Scale", _scale);
            //output.Write("Rotation", _rotation);
            //output.Write("Translation", _translation);

            throw new System.NotImplementedException(); // Is required?
        }

        /// <summary>
        /// Deserializes the transform.
        /// </summary>
        /// <param name="input">Input to read from</param>
        public void Read(/*ISavableReader input*/)
        {
            //_scale = input.ReadVector3();
            //_rotation = input.ReadQuaternion();
            //_translation = input.ReadVector3();
            ////Cache the matrix
            //Matrix4 m = this.Matrix;

            throw new System.NotImplementedException(); // Is required?
        }
    }
}
