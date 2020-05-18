using System.Collections.Generic;

namespace NthDimension.Network
{
    public enum enuAvatarSex
    {
        Male,
        Female,
        Random
    }



    public class AvatarInfoDesc
    {
        [System.Flags]
        public enum AvatarStates
        {
            AvatarIdle          = 2,
            AvatarWalking       = 4,
            AvatarRunning       = 8,
            AvatarDancing       = 16,
            AvatarTalking       = 32,
            AvatarSleeping      = 64,
        }

        public static Algebra.Vector3 Vector3FromVector3(Algebra.Vector3 vec)
        {
            return new Algebra.Vector3(vec.X, vec.Y, vec.Z);
        }

        public class AvatarTransformation
        {
            
            public Algebra.Vector3 Size         { get; set; }
            public Algebra.Vector3 SpawnPosition { get; set; }
            public Algebra.Matrix4 Orientation { get; set; }

            public AvatarTransformation(Algebra.Vector3 size, Algebra.Matrix4 orientation, Algebra.Vector3 spawn)
            {
                Size            = size;
                SpawnPosition   = SpawnPosition;
                Orientation     = orientation;
            }
        }

        public string AvatarName;
        public readonly enuAvatarSex AvatarSex;
        public readonly string UserId;
        public readonly string BodyType;
        public readonly string FaceType;
        public readonly string BodyMesh;
        public readonly string BodyMaterial;
        public readonly string BodySize;
        public readonly string BodyRotation;
        public readonly string SpawnAt;

        /// <summary>
        /// Holds attached models information as follows: 0 is Face, 1 is Hair, 2 is Shirt, 3 is Pants, 4 is Shoes
        /// </summary>
        public readonly List<AttachmentInfoDesc> Attachments = new List<AttachmentInfoDesc>();

        public AvatarInfoDesc(string userId, enuAvatarSex avatarSex, string bodyType, string faceType, string avatarName, string bodyMesh, string bodyMaterial, string bodySize, string bodyRotation, string spawnAt, List<AttachmentInfoDesc> attachments)
        {
            this.UserId = userId;
            this.AvatarName = avatarName;
            this.AvatarSex = avatarSex;
            this.BodyType = bodyType;
            this.FaceType = faceType;
            this.BodyMesh = bodyMesh;
            this.BodyMaterial = bodyMaterial;
            this.BodySize = bodySize;
            this.BodyRotation = bodyRotation;
            this.SpawnAt = spawnAt;
            this.Attachments = attachments;
        }
        public AvatarInfoDesc(string userId, string avatarSex, string bodyType, string faceType, string avatarName, string bodyMesh, string bodyMaterial, string bodySize, string bodyRotation, string spawnAt, List<AttachmentInfoDesc> attachments)
        {
            this.UserId = userId;
            this.AvatarName = avatarName;
            this.AvatarSex = avatarSex.ToLower() == "male" ? enuAvatarSex.Male : (avatarSex.ToLower() == "female" ? enuAvatarSex.Female : enuAvatarSex.Random);
            this.BodyType = bodyType;
            this.FaceType = faceType;
            this.BodyMesh = bodyMesh;
            this.BodyMaterial = bodyMaterial;
            this.BodySize = bodySize;
            this.BodyRotation = bodyRotation;
            this.SpawnAt = spawnAt;
            this.Attachments = attachments;
        }


        //public AvatarInfoDesc(string userId, enuAvatarSex avatarSex, string bodyType, string faceType, string avatarName, string bodyMesh, string bodyMaterial, string bodySize, string bodyRotation, string spawnAt, List<AttachmentInfoDesc> attachments, Algebra.Vector3 spawn):
        //    this(userId, avatarSex, bodyType, faceType, avatarName, bodyMesh, bodyMaterial, bodySize, bodyRotation, string.Format("{0}|{1}|{2}", spawn.X, spawn.Y, spawn.Z), attachments)
        //{

        //}
    }

    public class AttachmentInfoDesc
    {
        public string Name;
        public string Matrix;
        public string Vertex;
        public string Offset;
        public string Size;
        public string Orientation;
        public string Mesh;
        public string Material;
        public string Animation_Idle;
        public string Animation_Walk;
        public string Animation_Sit;
        public string Animation_SitIdle;
        public string Animation_Stand;
        public string Animation_Wave;

        public AttachmentInfoDesc() { }
        public AttachmentInfoDesc(AttachmentInfoDesc desc)
        {
            Name = desc.Name;
            Matrix = desc.Matrix;
            Vertex = desc.Vertex;
            Offset = desc.Offset;
            Size = desc.Size;
            Orientation = desc.Orientation;
            Mesh = desc.Mesh;
            Material = desc.Material;
            Animation_Idle = desc.Animation_Idle;
            Animation_Sit = desc.Animation_Sit;
            Animation_SitIdle = desc.Animation_SitIdle;
            Animation_Stand = desc.Animation_Stand;
            Animation_Walk = desc.Animation_Walk;
            Animation_Wave = desc.Animation_Wave;
        }
    }
}
