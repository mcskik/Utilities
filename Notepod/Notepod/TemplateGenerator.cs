using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Template generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class TemplateGenerator : BaseGenerator
    {
        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public TemplateGenerator(string[] inputLines) : base(inputLines)
        {
        }    
        #endregion

        #region Public methods.
        /// <summary>
        /// Import global parameters only.
        /// </summary>
        public override void Import()
        {
            ImportGlobalParameters();
        }

        /// <summary>
        /// Generate template matching shortcut key parameter.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            if (!_topLineParameters)
            {
                _shortcutKey = "Z";
            }
            switch (_shortcutKey)
            {
                case "B":
                    return GenerateTableBackoutTemplate();
                case "G":
                    return GenerateGenerateMethodTemplate();
                case "K":
                    return GenerateMockEntityTemplate();
                case "L":
                    return GenerateEntityListTemplate();
                case "M":
                    return GenerateMemberVariableToPropertyTemplate();
                case "N":
                    return GenerateTableForeignKeysTemplate();
                case "O":
                    return GenerateNewMockEntityTemplate();
                case "P":
                    return GenerateStripPropertiesTemplate();
                case "R":
                    return GenerateEnumerationMapperTemplate();
                case "S":
                    return GenerateCompareSameTemplate();
                case "U":
                    return GenerateConstructorTemplate();
                case "V":
                    return GenerateValidatorTemplate();
                case "X":
                    return GenerateMemberVariableToNewPropertySyntaxTemplate();
                case "Z":
                    return GenerateTemplateTemplate();
                default:
                    string[] messages = new string[1];
                    messages[0] = @"Unknown template key: """ + _shortcutKey + @"""";
                    return messages;
            }
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Generate table backout template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateTableForeignKeysTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Create Table Statement with foreign keys.";
            output.Add(line);
            line = @"USE [MyDatabase]";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"/****** Object:  Table [dbo].[tblMyTable]    Script Date: DD/MM/YYYY HH:NN:SS ******/";
            output.Add(line);
            line = @"SET ANSI_NULLS ON";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"SET QUOTED_IDENTIFIER ON";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"CREATE TABLE [dbo].[tblMyTable](";
            output.Add(line);
            line = @"[ID] [int] IDENTITY(1,1) NOT NULL,";
            output.Add(line);
            line = @"[Key] [int] NOT NULL,";
            output.Add(line);
            line = @"[MyTableForeignKey] [int] NOT NULL";
            output.Add(line);
            line = @"CONSTRAINT [MyTable_PK0001] PRIMARY KEY CLUSTERED";
            output.Add(line);
            line = @"(";
            output.Add(line);
            line = @"[ID] ASC";
            output.Add(line);
            line = @")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]";
            output.Add(line);
            line = @") ON [PRIMARY]";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"SET ANSI_PADDING OFF";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"ALTER TABLE [dbo].[tblMyTable]  WITH CHECK ADD  CONSTRAINT [FK_tblMyTable_tblParentTable] FOREIGN KEY([MyTableForeignKey])";
            output.Add(line);
            line = @"REFERENCES [dbo].[tblParentTable] ([ParentTableKey])";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"ALTER TABLE [dbo].[tblMyTable] CHECK CONSTRAINT [FK_tblMyTable_tblParentTable]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate table backout template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateTableBackoutTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Create Table Statement.[nnnn]";
            output.Add(line);
            line = @"USE [MyDatabase]";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"/****** Object:  Table [dbo].[tblMyTable]    Script Date: DD/MM/YYYY HH:NN:SS ******/";
            output.Add(line);
            line = @"SET ANSI_NULLS ON";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"SET QUOTED_IDENTIFIER ON";
            output.Add(line);
            line = @"GO";
            output.Add(line);
            line = @"CREATE TABLE [dbo].[tblMyTable](";
            output.Add(line);
            line = @"[ID] [int] IDENTITY(1,1) NOT NULL,";
            output.Add(line);
            line = @"[Key] [int] NOT NULL";
            output.Add(line);
            line = @"CONSTRAINT [MyTable_PK0001] PRIMARY KEY CLUSTERED";
            output.Add(line);
            line = @"(";
            output.Add(line);
            line = @"[ID] ASC";
            output.Add(line);
            line = @")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]";
            output.Add(line);
            line = @") ON [PRIMARY]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate member variable to property template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateMemberVariableToPropertyTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Member variables.";
            output.Add(line);
            line = @"private string _firstName;";
            output.Add(line);
            line = @"private string _lastName;";
            output.Add(line);
            line = @"private int _count;";
            output.Add(line);
            line = @"private readonly string _christianName;";
            output.Add(line);
            line = @"private readonly string _surname;";
            output.Add(line);
            line = @"private readonly int _frequency;";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate member variable to new property syntax template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateMemberVariableToNewPropertySyntaxTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Member variables.";
            output.Add(line);
            line = @"private string _firstName;";
            output.Add(line);
            line = @"private string _lastName;";
            output.Add(line);
            line = @"private int _count;";
            output.Add(line);
            line = @"private readonly string _christianName;";
            output.Add(line);
            line = @"private readonly string _surname;";
            output.Add(line);
            line = @"private readonly int _frequency;";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate entity attribute to property template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEntityAttributeToPropertyTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"public string";
            output.Add(line);
            line = @"billingAddress Notes:  House number or house name part of the billing address if the billing address details must be validated.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"billingPostCode Notes:  Post code part of the billing address if the billing address details must be validated.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"cardHolderName Notes:  The name as it appears on the payment card.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"cardNumber Notes:  The number on the front of the payment card (e.g., 4444333322221111 for a credit card) in encrypted form.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"cardSecurityCode Notes:  The 3-digit code on the back of the payment card in encrypted form.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"expiryDate Notes:  The expiry date of the payment card in MMYY format.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"public string";
            output.Add(line);
            line = @"issueNumber Notes:  The issue number that appears on certain cards, such as Switch cards.";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate entity association to property template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEntityAssociationToPropertyTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"«entity» FullDeliveryDetailsEntity";
            output.Add(line);
            line = @"Class    Name:";
            output.Add(line);
            line = @"Name: deliveryDetails";
            output.Add(line);
            line = @"The full delivery information recorded in the contract.";
            output.Add(line);
            line = @"«entity» PaymentSummaryEntity";
            output.Add(line);
            line = @"Class    Name:";
            output.Add(line);
            line = @"Name: paymentSummary";
            output.Add(line);
            line = @"The payment summary totals recorded in the contract.";
            output.Add(line);
            line = @"«entity» PaymentSummaryEntity";
            output.Add(line);
            line = @"Class    Name:";
            output.Add(line);
            line = @"Name: paymentSummary";
            output.Add(line);
            line = @"The order summary and payment summary totals recorded in the contract.";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate enumeration template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEnumerationTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"RewardType : public <<enumeration>> class Created: DD/MM/YYYY HH:NN:SS";
            output.Add(line);
            line = @"Modified: DD/MM/YYYY HH:NN:SS";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"Project:";
            output.Add(line);
            line = @"Author: Kenneth McSkimming";
            output.Add(line);
            line = @"Version: ?";
            output.Add(line);
            line = @"Phase: ?";
            output.Add(line);
            line = @"Status: ?";
            output.Add(line);
            line = @"Complexity: ?";
            output.Add(line);
            line = @"Advanced:";
            output.Add(line);
            line = @"UUID: {?}";
            output.Add(line);
            line = @"Appears In: ?";
            output.Add(line);
            line = @"Defines the different types of reward that a customer can get from a supplementary payment item.";
            output.Add(line);
            line = @"Attributes Other Links Attribute Details";
            output.Add(line);
            line = @"public";
            output.Add(line);
            line = @"Monetary <<enum>>";
            output.Add(line);
            line = @"Notes:  Monetary reward.";
            output.Add(line);
            line = @"";
            output.Add(line);
            line = @"private";
            output.Add(line);
            line = @"ClubCardPoints <<enum>>";
            output.Add(line);
            line = @"Notes:  Reward in the form of ClubCard points.";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate enumeration mapper template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEnumerationMapperTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@L=AS]";
            output.Add(line);
            line = @"public enum UpperLayerType : int";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"Monetary,";
            output.Add(line);
            line = @"ClubCardPoints";
            output.Add(line);
            line = @"}";
            output.Add(line);
            line = @"public enum LowerLayer.LowerLayerType : int";
            output.Add(line);
            line = @"{";
            output.Add(line);
            line = @"Money,";
            output.Add(line);
            line = @"ClubCard";
            output.Add(line);
            line = @"}";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate constructor template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateConstructorTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Member variables.[UpperLayerEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[LowerLayerEntities.LowerLayerEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate validator template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateValidatorTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Properties.[UpperLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[LowerLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate entity list template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateEntityListTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@L=AS]";
            output.Add(line);
            line = @"public class SampleEntities : List<SampleEntity>";
            output.Add(line);
            return output.ToArray();
        }
        
        /// <summary>
        /// Generate generate method template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateGenerateMethodTemplate()
        {
            string[] output = new string[1];
            output[0] = @"Paste in a sample method here to create a starting point generator method for it.";
            return output;
        }

        /// <summary>
        /// Generate compare same template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateCompareSameTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Properties.[UpperLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[LowerLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }
 
        /// <summary>
        /// Generate mock entity template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateMockEntityTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@L=AS]";
            output.Add(line);
            line = @"#region Properties.[UpperLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[LowerLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate new mock entity template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateNewMockEntityTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Properties.[Entity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate strip properties template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateStripPropertiesTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@S=P]";
            output.Add(line);
            line = @"#region Properties.[UpperLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[LowerLayerEntities.SampleEntity]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate template template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateTemplateTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@K=U]";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate activity wrapper template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateActivityWrapperTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"#region Member variables.[ActivityRequest]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[ActivityRequest]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }

        /// <summary>
        /// Generate TypeMock method template.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateTypeMockMethodTemplate()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"[@NAMESPACE=""Customer""]";
            output.Add(line);
            line = @"#region Member variables.[ActivityRequest]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            line = @"#region Properties.[ActivityRequest]";
            output.Add(line);
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }
        #endregion
    }
}