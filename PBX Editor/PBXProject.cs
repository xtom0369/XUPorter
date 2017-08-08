using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXProject : PBXObject
	{
		protected string MAINGROUP_KEY = "mainGroup";
		protected string KNOWN_REGIONS_KEY = "knownRegions";
        protected string ATTRIBUTES_KEY = "attributes";
        protected string TARGETS_KEY = "targets";

        protected string PUSH_NOTIFICATIONS = "com.apple.Push";
        protected string InAppPurchase = "com.apple.InAppPurchase";
        protected string BackgroundModes = "com.apple.BackgroundModes";

		protected bool _clearedLoc = false;

		public PBXProject() : base() {
		}
		
		public PBXProject( string guid, PBXDictionary dictionary ) : base( guid, dictionary ) {
		}
		
		public string mainGroupID {
			get {
				return (string)_data[ MAINGROUP_KEY ];
			}
		}

		public PBXList knownRegions {
			get {
				return (PBXList)_data[ KNOWN_REGIONS_KEY ];
			}
		}

        public PBXDictionary attributes
        {
            get
            {
                return (PBXDictionary)_data[ATTRIBUTES_KEY];
            }
        }

        public PBXList targets
        {
            get
            {
                return (PBXList)_data[TARGETS_KEY];
            }
        }

		public void AddRegion(string region) {
			if (!_clearedLoc)
			{
				// Only include localizations we explicitly specify
				knownRegions.Clear();
				_clearedLoc = true;
			}

			knownRegions.Add(region);
		}

        public void SetPushNotificationAttrEnable(bool enable)
        {
            int value = enable ? 1 : 0;
            SetSystemAttributeValue(PUSH_NOTIFICATIONS, value);
        }

        public void SetInAppPurchaseAttrEnable(bool enable)
        {
            int value = enable ? 1 : 0;
            SetSystemAttributeValue(InAppPurchase, value);
        }

        public void SetBackgroundModesAttrEnable(bool enable)
        {
            int value = enable ? 1 : 0;
            SetSystemAttributeValue(BackgroundModes, value);
        }

        void SetSystemAttributeValue(string attrName, int value)
        {
            string TargetAttrKey = "TargetAttributes";
            if (attributes.ContainsKey(TargetAttrKey))
            {
                PBXDictionary targetAttrs = (PBXDictionary)attributes[TargetAttrKey];

                foreach (object target in targets)
                {
                    string id = target.ToString();
                    if (!targetAttrs.ContainsKey(id))
                    {
                        PBXDictionary targetAttr = new PBXDictionary();
                        targetAttr["SystemCapabilities"] = new PBXDictionary();
                        targetAttrs[id] = targetAttr;
                    }
                }

                PBXDictionary.ValueCollection.Enumerator it = targetAttrs.Values.GetEnumerator();
                while (it.MoveNext())
                {
                    PBXDictionary subAttrs = it.Current as PBXDictionary;
                    if (subAttrs != null && subAttrs.ContainsKey("SystemCapabilities"))
                    {
                        PBXDictionary capabilities = (PBXDictionary)subAttrs["SystemCapabilities"];
                        PBXDictionary attrValue = new PBXDictionary();
                        attrValue["enabled"] = value;
                        capabilities[attrName] = attrValue;
                    }
                }
            }
        }
	}
}
