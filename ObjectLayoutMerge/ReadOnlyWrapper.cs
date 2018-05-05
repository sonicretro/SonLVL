using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace ObjectLayoutMerge
{
	public class ReadOnlyWrapper : ICustomTypeDescriptor
	{
		private object wrapObj;
		private ReadOnlyConverter converter;

		public ReadOnlyWrapper(object wrapObj)
		{
			this.wrapObj = wrapObj;
			converter = new ReadOnlyConverter(TypeDescriptor.GetConverter(wrapObj));
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(wrapObj);

		string ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(wrapObj);

		string ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(wrapObj);

		TypeConverter ICustomTypeDescriptor.GetConverter() => converter;

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(wrapObj);

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(wrapObj);

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(wrapObj, editorBaseType);

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(wrapObj);

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(wrapObj, attributes);

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => TypeDescriptor.GetProperties(wrapObj);

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(wrapObj, attributes);
			PropertyDescriptor[] result = new PropertyDescriptor[props.Count];
			for (int i = 0; i < props.Count; i++)
				result[i] = new ReadOnlyDescriptor(props[i]);
			return new PropertyDescriptorCollection(result);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => wrapObj;
	}

	public class ReadOnlyConverter : TypeConverter
	{
		private TypeConverter typeConverter;

		public ReadOnlyConverter(TypeConverter typeConverter)
		{
			this.typeConverter = typeConverter;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => typeConverter.CanConvertFrom(context, sourceType);

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => typeConverter.CanConvertTo(context, destinationType);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => typeConverter.ConvertFrom(context, culture, value);

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => typeConverter.ConvertTo(context, culture, value, destinationType);

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) => typeConverter.CreateInstance(context, propertyValues);

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => typeConverter.GetCreateInstanceSupported(context);

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection props = typeConverter.GetProperties(context, value, attributes);
			PropertyDescriptor[] result = new PropertyDescriptor[props.Count];
			for (int i = 0; i < props.Count; i++)
				result[i] = new ReadOnlyDescriptor(props[i]);
			return new PropertyDescriptorCollection(result);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context) => typeConverter.GetPropertiesSupported(context);

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => typeConverter.GetStandardValues(context);

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => typeConverter.GetStandardValuesExclusive(context);

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => typeConverter.GetStandardValuesSupported(context);

		public override bool IsValid(ITypeDescriptorContext context, object value) => typeConverter.IsValid(context, value);
	}

	public class ReadOnlyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor propertyDescriptor;
		private TypeConverter typeConverter;

		public ReadOnlyDescriptor(PropertyDescriptor propertyDescriptor) : base(propertyDescriptor)
		{
			this.propertyDescriptor = propertyDescriptor;
			typeConverter = new ReadOnlyConverter(propertyDescriptor.Converter);
		}

		public override Type ComponentType => propertyDescriptor.ComponentType;

		public override bool IsReadOnly => true;

		public override Type PropertyType => propertyDescriptor.PropertyType;

		public override bool CanResetValue(object component) => false;

		public override object GetValue(object component) => propertyDescriptor.GetValue(component);

		public override void ResetValue(object component) { }

		public override void SetValue(object component, object value) { }

		public override bool ShouldSerializeValue(object component) => propertyDescriptor.ShouldSerializeValue(component);

		public override TypeConverter Converter => typeConverter;
	}
}
