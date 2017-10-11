﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;

namespace BizTalk.PipelineComponents
{

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("9FA416D7-88F6-4005-885D-8CB3AC56AF41")]
    public class AddContextProperties : IBaseComponent, IComponent, IComponentUI, IPersistPropertyBag
    {

        #region Properties

        


        private ContextValueCollection properties;


        public AddContextProperties()
        {
          
            ContextValueCollection c = new ContextValueCollection();
            c.Dirty += new IsDirty(DirtyFired);
            Properties = c;
        }

        void DirtyFired()
        {
            Properties = properties;
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Editor(typeof(ContextValueCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ContextValueCollection Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Context values not specified");
                this.properties = value;
               
            }

        }
       
       
        

        #endregion

        #region IBaseComponent Members
        [System.ComponentModel.Browsable(false)]
        public string Description
        {
            get
            {
                return "Context Adder";
            }
        }
        [System.ComponentModel.Browsable(false)]
        public string Name
        {
            get
            {
                return "ContextAdder";
            }
        }
        [System.ComponentModel.Browsable(false)]
        public string Version
        {
            get
            {
                return "1.0.0.0";
            }
        }

        #endregion

        #region IComponent Members

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
           

            IEnumerator e = Properties.GetEnumerator();
            while (e.MoveNext())
            {
                ContextValue v = (ContextValue)e.Current;

                //2017-09-09 Added possibility to use expression instead of a static value
                object obj = ScriptExpressionHelper.Expression(v.Value);
                var value = Convert.ChangeType(obj, v.Code);

                pInMsg.Context.Promote(v.Key, v.Namespace, value);

            }
            return pInMsg;
        }

        #endregion

        #region IComponentUI Members

        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return null;
        }
        [System.ComponentModel.Browsable(false)]
        public System.IntPtr Icon
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        #endregion

        #region IPersistPropertyBag Members

        public void InitNew()
        {
        }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("9FA416D7-88F6-4005-885D-8CB3AC56AF41");
        }

        object ReadProperty(IPropertyBag propertyBag, string key)
        {
            object val = null;
            try
            {
                propertyBag.Read(key, out val, 0);
            }
            catch
            {
            }
            return val;

        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {

              object val = ReadProperty(propertyBag, "Properties");

            if(val != null)
                Properties.SetCollection((string)val);
        
            
        }


        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {

            propertyBag.Write("Properties", Properties.ToString());
                      

        }

        #endregion
    }
}
