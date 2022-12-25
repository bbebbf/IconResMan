using System.Runtime.InteropServices;

namespace IconResMan
{
    public sealed class ResourceName : CachedIntPtr
    {
        public ResourceName(string name)
        {
            Name = name;
        }

        public ResourceName(ushort id)
        {
            Id = id;
        }

        public ResourceName(IntPtr name_or_id)
        {
            var name = Marshal.PtrToStringUni(name_or_id);
            if (name == null)
            {
                Id = (ushort)name_or_id;
            }
            else
            {
                Name = name;
            }
        }

        public ResourceName(ResourceName name)
        {
            Name = name.Name;
            Id = name.Id;
        }

        public ResourceName(ArgumentToResNameAdapter argument)
        {
            if (argument.IdFound)
            {
                Id = argument.Id;
            }
            else
            {
                Name = argument.Argument;
            }
        }

        public string Title()
        {
            if (Name != null)
            {
                return "Name";
            }
            else if (Id != null)
            {
                return "Id";
            }
            return string.Empty;
        }

        public ushort? Id { get; init; }

        public string? Name { get; init; }

        protected override void GetCachedIntPtr(ref IntPtr handle, ref bool needsToBeReleased)
        {
            if (Name != null)
            {
                handle = Marshal.StringToHGlobalUni(Name);
                needsToBeReleased = true;

            }
            else if (Id != null && Id > 0)
            {
                handle = (IntPtr)Id;
                needsToBeReleased = false;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not ResourceName)
                return base.Equals(obj);

            var objName = (ResourceName)obj;
            if (objName.Name != null && this.Name != null)
            {
                return objName.Name.Equals(this.Name);
            }
            else if (objName.Id != null && this.Id != null)
            {
                return objName.Id.Equals(this.Id);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Name != null)
            {
                return Name.GetHashCode();
            }
            else if (Id != null)
            {
                return Id.GetHashCode();
            }
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            if (Name != null)
            {
                return $"\"{Name}\"";
            }
            else if (Id != null)
            {
                return "#" + Id;
            }
            return base.ToString();
        }
    }
}
