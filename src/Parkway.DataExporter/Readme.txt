1) Copy DownloadExporterSettings xml file into the application that will use the export library

2) Add this line of code to the appsettings of the config of the application that will use the export library
    <add key="DownloadTemplatesConfigFile" value="~/DownloadExporterSettings.xml" />

	value is the file path of the DownloadExporterSettings xml file
