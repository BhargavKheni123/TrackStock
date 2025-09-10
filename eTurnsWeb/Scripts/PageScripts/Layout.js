var CurrentURL = window.location.href;
if (CurrentURL.indexOf("localhost:") < 0 && CurrentURL.toLowerCase().indexOf("demo") < 0)
{
    
    if(CurrentURL.indexOf("http:") >= 0)
    {
        CurrentURL= CurrentURL.replace("http:", "https:");
        window.location.href = CurrentURL;
    }
}