using System;
using System.IO;
using Structurizr;
using Structurizr.Api;
using Structurizr.Documentation;

namespace architecture
{
    class Program
    {
        private const long WorkspaceId = 1;
        private const string ApiKey = @"";
        private const string ApiSecret = @"";
        private const string AlertTag = @"";

        static void Main(string[] args)
        {
            Console.WriteLine("Creating diagrams");

            var workspace = CreateWorkspace();

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }

        public static Workspace CreateWorkspace()
        {
            var workspace = new Workspace("LIGHT P&D", "Projeto P&D Realidade Aumentada Light");

            var model = workspace.Model;

            SoftwareSystem financialRiskSystem = model.AddSoftwareSystem("Financial Risk System", "Calculates the bank's exposure to risk for product X.");

            Person businessUser = model.AddPerson("Business User", "A regular business user.");
            businessUser.Uses(financialRiskSystem, "Views reports using");

            Person configurationUser = model.AddPerson("Configuration User", "A regular business user who can also configure the parameters used in the risk calculations.");
            configurationUser.Uses(financialRiskSystem, "Configures parameters using");

            SoftwareSystem tradeDataSystem = model.AddSoftwareSystem("Trade Data System", "The system of record for trades of type X.");
            financialRiskSystem.Uses(tradeDataSystem, "Gets trade data from");

            SoftwareSystem referenceDataSystem = model.AddSoftwareSystem("Reference Data System", "Manages reference data for all counterparties the bank interacts with.");
            financialRiskSystem.Uses(referenceDataSystem, "Gets counterparty data from");

            SoftwareSystem referenceDataSystemV2 = model.AddSoftwareSystem("Reference Data System v2.0", "Manages reference data for all counterparties the bank interacts with.");
            referenceDataSystemV2.AddTags("Future State");
            financialRiskSystem.Uses(referenceDataSystemV2, "Gets counterparty data from").AddTags("Future State");

            SoftwareSystem emailSystem = model.AddSoftwareSystem("E-mail system", "The bank's Microsoft Exchange system.");
            financialRiskSystem.Uses(emailSystem, "Sends a notification that a report is ready to");
            emailSystem.Delivers(businessUser, "Sends a notification that a report is ready to", "E-mail message", InteractionStyle.Asynchronous);

            SoftwareSystem centralMonitoringService = model.AddSoftwareSystem("Central Monitoring Service", "The bank's central monitoring and alerting dashboard.");
            financialRiskSystem.Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            SoftwareSystem activeDirectory = model.AddSoftwareSystem("Active Directory", "The bank's authentication and authorisation system.");
            financialRiskSystem.Uses(activeDirectory, "Uses for user authentication and authorisation");

            ViewSet views = workspace.Views;
            SystemContextView contextView = views.CreateSystemContextView(financialRiskSystem, "Context", "An example System Context diagram for the Financial Risk System architecture kata.");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            Styles styles = views.Configuration.Styles;
            financialRiskSystem.AddTags("Risk System");

            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff", FontSize = 34 });
            styles.Add(new ElementStyle("Risk System") { Background = "#550000", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Width = 650, Height = 400, Background = "#801515", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Width = 550, Background = "#d46a6a", Shape = Shape.Person });

            styles.Add(new RelationshipStyle(Tags.Relationship) { Thickness = 4, Dashed = false, FontSize = 32, Width = 400 });
            styles.Add(new RelationshipStyle(Tags.Synchronous) { Dashed = false });
            styles.Add(new RelationshipStyle(Tags.Asynchronous) { Dashed = true });
            styles.Add(new RelationshipStyle(AlertTag) { Color = "#ff0000" });

            styles.Add(new ElementStyle("Future State") { Opacity = 30, Border = Border.Dashed });
            styles.Add(new RelationshipStyle("Future State") { Opacity = 30, Dashed = true });

            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);
            DirectoryInfo documentationRoot = new DirectoryInfo("FinancialRiskSystem");
            template.AddContextSection(financialRiskSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "context.adoc")));
            template.AddFunctionalOverviewSection(financialRiskSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "functional-overview.md")));
            template.AddQualityAttributesSection(financialRiskSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "quality-attributes.md")));
            template.AddImages(documentationRoot);
            
            return workspace;
        }
    }
}
