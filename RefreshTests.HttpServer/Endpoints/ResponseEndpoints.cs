using System.Net;
using Refresh.HttpServer.Endpoints;
using Refresh.HttpServer.Responses;
using RefreshTests.HttpServer.Tests;

namespace RefreshTests.HttpServer.Endpoints;

public class ResponseEndpoints : EndpointGroup
{
    [Endpoint("/response/string")]
    public string String(HttpListenerContext context)
    {
        return "works";
    }
    
    [Endpoint("/response/responseObject")]
    public Response ResponseObject(HttpListenerContext context)
    {
        return new Response("works", ContentType.Plaintext);
    }
    
    [Endpoint("/response/responseObjectWithCode")]
    public Response ResponseObjectWithCode(HttpListenerContext context)
    {
        return new Response("works", ContentType.Plaintext, HttpStatusCode.Accepted); // random code lol
    }

    [Endpoint("/response/serializedXml", Method.Get, ContentType.Xml)]
    [Endpoint("/response/serializedJson", Method.Get, ContentType.Json)]
    public ResponseSerializationObject SerializedObject(HttpListenerContext context)
    {
        return new ResponseSerializationObject();
    }
}