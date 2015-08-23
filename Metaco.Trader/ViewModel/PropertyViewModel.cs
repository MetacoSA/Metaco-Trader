using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using PowerWallet.Controls;

namespace PowerWallet.ViewModel
{
    public class PropertyBuilder
    {
        PropertyViewModel _Parent;
        public PropertyBuilder(PropertyViewModel parent, string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _Parent = parent;
            _Name = name;
        }

        string _Name;

        List<Attribute> _Attributes = new List<Attribute>();
        public PropertyBuilder AddAttributes(params Attribute[] attributes)
        {
            _Attributes.AddRange(attributes);
            return this;
        }

        public PropertyBuilder SetEditor(Type editorType)
        {
            return AddAttributes(new EditorAttribute(editorType, editorType));
        }

        public PropertyViewModel.PropertyViewModelPropertyDescriptor Commit()
        {
            var property = new PropertyViewModel.PropertyViewModelPropertyDescriptor(_Name, _Parent, _Attributes.ToArray());
            _Parent._Properties.Add(property);
            return property;
        }

        public PropertyBuilder SetDisplay(string name)
        {
            return AddAttributes(new DisplayNameAttribute(name));
        }

        public PropertyBuilder SetCategory(string name)
        {
            return AddAttributes(new CategoryAttribute(name));
        }
    }
    public class PropertyViewModel : CustomTypeDescriptor
    {
        public class PropertyPropertyDescriptor : PropertyDescriptor
        {
            private PropertyInfo prop;
            private PropertyViewModel parent;

            public PropertyPropertyDescriptor(PropertyInfo prop, PropertyViewModel propertyViewModel)
                : base(prop.Name, prop.GetCustomAttributes().OfType<Attribute>().ToArray())
            {
                this.prop = prop;
                this.parent = propertyViewModel;
            }
            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get
                {
                    return prop.DeclaringType;
                }
            }

            public override object GetValue(object component)
            {
                return prop.GetValue(component);
            }

            public override bool IsReadOnly
            {
                get
                {
                    return !prop.CanWrite;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return prop.PropertyType;
                }
            }

            public override void ResetValue(object component)
            {

            }

            public override void SetValue(object component, object value)
            {
                prop.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }
        public class PropertyViewModelPropertyDescriptor : PropertyDescriptor
        {
            public PropertyViewModelPropertyDescriptor(string name, object component, Attribute[] attributes)
                : base(name, attributes)
            {
                _component = component;
            }

            public PropertyViewModelPropertyDescriptor SetValue(object value)
            {
                _Value = value;
                return this;
            }

            object _component;
            object _Value;
            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get
                {
                    return _component.GetType();
                }
            }

            public override object GetValue(object component)
            {
                return _Value;
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return _Value.GetType();
                }
            }

            public override void ResetValue(object component)
            {
                throw new NotSupportedException();
            }

            public override void SetValue(object component, object value)
            {
                SetValue(value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }
        internal List<PropertyViewModelPropertyDescriptor> _Properties = new List<PropertyViewModelPropertyDescriptor>();

        Random _Rand = new Random();
        public PropertyBuilder NewProperty()
        {
            return new PropertyBuilder(this, "gen_" + _Rand.Next());
        }
        public PropertyBuilder NewProperty(string name)
        {
            return new PropertyBuilder(this, name);
        }


        public override PropertyDescriptorCollection GetProperties()
        {
            List<PropertyDescriptor> baseProperties = new List<PropertyDescriptor>();
            foreach (var prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                baseProperties.Add(new PropertyPropertyDescriptor(prop, this));
            }
            PropertyDescriptor[] instanceProperties = _Properties.OfType<PropertyDescriptor>().ToArray();
            var result = instanceProperties.Concat(baseProperties.OfType<PropertyDescriptor>()).ToArray();
            return new PropertyDescriptorCollection(result);
        }
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }
    }
}
