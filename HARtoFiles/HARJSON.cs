
using System;

public class Rootobject
{
    public Log log { get; set; }
}

public class Log
{
    public string version { get; set; }
    public Creator creator { get; set; }
    public Browser browser { get; set; }
    public Page[] pages { get; set; }
    public Entry[] entries { get; set; }
}

public class Creator
{
    public string name { get; set; }
    public string version { get; set; }
}

public class Browser
{
    public string name { get; set; }
    public string version { get; set; }
}

public class Page
{
    public DateTime startedDateTime { get; set; }
    public string id { get; set; }
    public string title { get; set; }
    public Pagetimings pageTimings { get; set; }
}

public class Pagetimings
{
    public int onContentLoad { get; set; }
    public int onLoad { get; set; }
}

public class Entry
{
    public string pageref { get; set; }
    public DateTime startedDateTime { get; set; }
    public Request request { get; set; }
    public Response response { get; set; }
    public Cache cache { get; set; }
    public Timings timings { get; set; }
    public int time { get; set; }
    public string _securityState { get; set; }
    public string serverIPAddress { get; set; }
    public string connection { get; set; }
}

public class Request
{
    public int bodySize { get; set; }
    public string method { get; set; }
    public string url { get; set; }
    public string httpVersion { get; set; }
    public Header[] headers { get; set; }
    public Cooky[] cookies { get; set; }
    public Querystring[] queryString { get; set; }
    public int headersSize { get; set; }
}

public class Header
{
    public string name { get; set; }
    public string value { get; set; }
}

public class Cooky
{
    public string name { get; set; }
    public string value { get; set; }
}

public class Querystring
{
    public string name { get; set; }
    public string value { get; set; }
}

public class Response
{
    public int status { get; set; }
    public string statusText { get; set; }
    public string httpVersion { get; set; }
    public Header1[] headers { get; set; }
    public object[] cookies { get; set; }
    public Content content { get; set; }
    public string redirectURL { get; set; }
    public int headersSize { get; set; }
    public int bodySize { get; set; }
}

public class Content
{
    public string mimeType { get; set; }
    public int size { get; set; }
    public string encoding { get; set; }
    public string text { get; set; }
}

public class Header1
{
    public string name { get; set; }
    public string value { get; set; }
}

public class Cache
{
}

public class Timings
{
    public int blocked { get; set; }
    public int dns { get; set; }
    public int connect { get; set; }
    public int ssl { get; set; }
    public int send { get; set; }
    public int wait { get; set; }
    public int receive { get; set; }
}
