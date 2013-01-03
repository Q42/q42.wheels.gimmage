using System;
using System.Configuration;

namespace Q42.Wheels.Gimmage.Config
{
  internal class SourceCollection : ConfigurationElementCollection
  {
    [ConfigurationProperty("default")]
    public string Default
    {
      get
      {
        if (ElementInformation.Properties["default"] != null)
          return ElementInformation.Properties["default"].Value as string;
        return null;
      }
    }

    public SourceElement DefaultSource
    {
      get
      {
        if (!String.IsNullOrEmpty(Default))
          return this[Default];

        return this[0];
      }
    }

    public SourceElement this[int index]
    {
      get
      {
        return BaseGet(index) as SourceElement;
      }
      set
      {
        if (BaseGet(index) != null)
          BaseRemoveAt(index);
        BaseAdd(index, value);
      }
    }

    public new SourceElement this[string index]
    {
      get
      {
        return BaseGet(index) as SourceElement;
      }
      set
      {
        if (BaseGet(index) != null)
          BaseRemove(index);
        BaseAdd(value);
      }
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return new SourceElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((SourceElement)element).Name;
    }
  }
}
