namespace Demo_ServiceCollectionClass.Model;

public class XMLWriter : IXMLWriter
{
    public void WriteXML()
    {
        Console.WriteLine("<message>Writing in XML Format</message>");
    }
}

public class JSONWriter : IJSONWriter
{
    public void WriteJSON()
    {
        Console.WriteLine("{'message': 'Writing in JSON Format'}");
    }
}