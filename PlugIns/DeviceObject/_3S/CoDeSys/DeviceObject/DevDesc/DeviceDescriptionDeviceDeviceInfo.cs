using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace _3S.CoDeSys.DeviceObject.DevDesc
{
    [Serializable]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.3s-software.com/schemas/DeviceDescription-1.0.xsd")]
    public class DeviceDescriptionDeviceDeviceInfo
    {
        private StringRefType nameField;

        private StringRefType descriptionField;

        private StringRefType vendorField;

        private string orderNumberField;

        private FileRefType imageField;

        private FileRefType iconField;

        private DeviceDescriptionDeviceDeviceInfoDefaultInstanceName defaultInstanceNameField;

        private int[] categoryField;

        private string[] familyField;

        private DeviceDescriptionDeviceDeviceInfoFile[] additionalFilesField;

        private CustomType customField;

        private DeviceDescriptionDeviceDeviceInfoSubCategory subCategoryField;

        private string[] hideCategoryField;

        private string vendorIdField;

        public StringRefType Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        public StringRefType Description
        {
            get
            {
                return descriptionField;
            }
            set
            {
                descriptionField = value;
            }
        }

        public StringRefType Vendor
        {
            get
            {
                return vendorField;
            }
            set
            {
                vendorField = value;
            }
        }

        public string OrderNumber
        {
            get
            {
                return orderNumberField;
            }
            set
            {
                orderNumberField = value;
            }
        }

        public FileRefType Image
        {
            get
            {
                return imageField;
            }
            set
            {
                imageField = value;
            }
        }

        public FileRefType Icon
        {
            get
            {
                return iconField;
            }
            set
            {
                iconField = value;
            }
        }

        public DeviceDescriptionDeviceDeviceInfoDefaultInstanceName DefaultInstanceName
        {
            get
            {
                return defaultInstanceNameField;
            }
            set
            {
                defaultInstanceNameField = value;
            }
        }

        [XmlElement("Category")]
        public int[] Category
        {
            get
            {
                return categoryField;
            }
            set
            {
                categoryField = value;
            }
        }

        [XmlElement("Family")]
        public string[] Family
        {
            get
            {
                return familyField;
            }
            set
            {
                familyField = value;
            }
        }

        [XmlArrayItem("File", IsNullable = false)]
        public DeviceDescriptionDeviceDeviceInfoFile[] AdditionalFiles
        {
            get
            {
                return additionalFilesField;
            }
            set
            {
                additionalFilesField = value;
            }
        }

        public CustomType Custom
        {
            get
            {
                return customField;
            }
            set
            {
                customField = value;
            }
        }

        public DeviceDescriptionDeviceDeviceInfoSubCategory SubCategory
        {
            get
            {
                return subCategoryField;
            }
            set
            {
                subCategoryField = value;
            }
        }

        [XmlElement("HideCategory")]
        public string[] HideCategory
        {
            get
            {
                return hideCategoryField;
            }
            set
            {
                hideCategoryField = value;
            }
        }

        [XmlAttribute]
        public string VendorId
        {
            get
            {
                return vendorIdField;
            }
            set
            {
                vendorIdField = value;
            }
        }
    }
}
