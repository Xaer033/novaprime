using System;
using UnityEngine;

namespace GhostGen
{
    public struct AssetRequest : IEquatable<AssetRequest>
    {
        public string path;
        public Transform parent;

        public AssetRequest(string pPath, Transform pParent = null)
        {
            path = pPath;
            parent = pParent;
        }

        public static bool IsSame(AssetRequest a, AssetRequest b)
        {
            return a.path == b.path &&
                    a.parent == b.parent;
        }

        public bool Equals(AssetRequest other)
        {
            return IsSame(this, other);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj))
                return false;
            if(obj.GetType() != typeof(AssetRequest))
                return false;
            return Equals((AssetRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (path.GetHashCode() * 397) ^ parent.GetHashCode();
            }
        }

        public static bool operator ==(AssetRequest left, AssetRequest right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetRequest left, AssetRequest right)
        {
            return !left.Equals(right);
        }
    }
}
