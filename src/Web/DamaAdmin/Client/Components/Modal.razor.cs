using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DamaAdmin.Client.Components
{
    public partial class Modal
    {
        public Guid Guid = Guid.NewGuid();
        public string ModalDisplay = "none;";
        public string ModalClass = "";
        public bool ShowBackdrop = false;
        public string ModalTitle = "";
        public List<(string DisplayText, string Value)> DisplayItems;
        public void Open<T>(string title, T model)
        {
            ModalTitle = title;
            DisplayItems = GetNamesAndValues(model);
            ModalDisplay = "block;";
            ModalClass = "Show";
            ShowBackdrop = true;
            StateHasChanged();
        }

        public void Close()
        {
            ModalDisplay = "none";
            ModalClass = "";
            ShowBackdrop = false;
            StateHasChanged();
        }

        public List<(string, string)> GetNamesAndValues<T>(T model)
        {
            var props = typeof(T).GetProperties();
            List<(string, string)> list = new();
            foreach (var prop in props)
            {
                if (prop != null)
                {
                    string displayName = prop.Name;
                    System.Reflection.MemberInfo propInfo = typeof(T).GetProperty(prop.Name);
                    if (propInfo != null)
                    {
                        if (propInfo.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute))is System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
                        {
                            displayName = $"{displayAttribute.Name} ";
                        }
                    }                    
                    //var val = prop.GetValue(model)?.ToString() ?? "";
                    var propValue = prop.GetValue(model);
                    string val = "";
                    if(propValue != null)
                    {
                        if (propValue is IEnumerable<int>)
                            val = PrintList(propValue as IEnumerable<int>);
                        else if (propValue is IEnumerable<string>)
                            val = PrintList(propValue as IEnumerable<string>);
                        else
                            val = propValue.ToString();
                    }
                    list.Add((displayName, val));
                }
            }
            
            return list;
        }        
        private string PrintList<T>(IEnumerable<T> list)
        {
            return string.Join(",", list.Select(i => string.Format("{0}", i)));
        }
    }
}