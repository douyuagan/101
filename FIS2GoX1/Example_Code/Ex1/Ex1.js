/*
 In this example we're going to:

 1) construct an object
 2) Send it to our web-service
 3) Prepare our UI for the response 
 4) Handle the Response
    4.1) Show the contents of our message
    4.2) Show exception information, if it occurs


   !==========================================
      A note about namespaces. Typically, when we write Javascript functions we end up with files that look like...


      function A()
      {
       ...
      }
      function B()
      {
       ...
      }

      Which is fine for utility files or prototypes, but is insufficient for building full-GUIs. Typically, we'll take advantage of Javascript
      namespaces for this, which encapsulates functions into an object which lives at the global level. In non-Angular JS that I write, you'll
      find I write code like the following

      var FIS = FIS || {};
      FIS.Example1 = {};

      FIS.Example1.ButtonClick = function() {

      };

      which creates 2 namespaces (FIS and FIS.Example1), and allows us to use generic function names like "Load()" or "Save()" and not have to 
      worry about having those overwritten in another javascript file. JQuery uses the "$" symbol as their namespace. Angular takes another approach,
      called closures, but I still recommend getting comfortable with the above.

*/



var FIS = FIS || {};    //this creates a global-object called "FIS". If FIS was already defined in another file, don't overwrite it.
FIS.Example1 = {};      //we know example1 namespace doesn't exist, so we'll create it from scratch here


FIS.Example1.Log = function (msg) { //a little logger function which adds a timestamp
    var d = new Date();
    console.log(d.toLocaleTimeString() + ": " + msg);
};
FIS.Example1.btnClick = function () {  //Define our button-handler logic

    //Step 1. We're going to create a Parameter object to send to our web-service
    // For Reference, our function is looking for the following: 
    //      public ComplexResponseObject Example1(int NumberToSquare, ComplexInputArgument cArg)
    // With a class-definition:
    //      public DateTime dt;
    //      public String Username;
    //

    var NumberToSquare = 6;          //set up our simple argument
    var cArg = {};                   //create an empty object for our complex input
    cArg.dt = new Date();            //Provide the date, be sure the name matches
    cArg.Username = "Todd";          //Generally, we can pass primitives like String, Int, Bool directly and we can pass Lists as Javascript arrays

    //package-up our input into a single object to pass to the AJAX function
    //we then "stringify" that object so it can be sent over the wire, essentially converting our
    //javascript object to a string
    var strQueryParams = JSON.stringify({ "NumberToSquare": NumberToSquare, "cArg": cArg }); 

    //Step 2. With our Inputs ready, we're going to send it over to the Web-Service, but first
    // we'll prepare functions that we want executed on specific events completing, these are called "Callbacks" because they 
    // aren't executed now, we "call back" to them later.

    //In javascript, functions are called "First-Class Objects", meaning that we can define them just like any other object, int, string, etc.
    // as simple variables in our code

    //First, What do we want to happen when the request is SENT
    var fnSent = function () {
        FIS.Example1.Log("Request Sent!"); //We'll Log it first

        //then, we want to clear-out the container which we're going to put our response in
        // quick explanation if you're new to jQuery
        // $("#result_container").html("");
        // $: means that whatever follows is jQuery, and the default is called a "selector" function, which selects objects from HTML
        // ("#result_container"): this is where we pass our selector. In this case, we are using the "#" search pattern, which tells jQuery to find an object with ID = result_container
        //      these patterns are defined in CSS and we can also use "." to find by CSS class or just plain "BUTTON" to find all HTML Buttons
        // .html(""): Anything after the select runs on the result of the selection, so in this case, we're using shorthand to set the innerHTML of that div to blank
        // 
        // the above command is equivalent to: 
        //  document.getElementById("result_container").innerHtml = "";
        //
        //https://api.jquery.com/category/selectors/

        $("#result_container").html("");


        //since the user might be waiting for a while, we'll add a loading indicator. this uses the "Font Awesome" icon library
        // to add a little spinning indicator
        // http://fontawesome.io/examples/

        $("#result_container").html('<div style="text-align:center"><i class="fa fa-cog fa-spin fa-5x fa-fw"></i><br />Loading...</div>');

    };

    //Second, What do we want to do when it fails?
    var fnFailed = function (obj) {

        FIS.Example1.Log("An error has occurred!");
        console.log(obj);

        //when the server returns HTTP 400 or 500 type error, the browser will tell your code that it failed and 
        //pass some generic information about what failed. We're going to look at that failure and present some information to the user

        $("#result_container").html(""); //clear out the container


        //I want to include some additional details about the error, so we're going to use the "append()" function to build-up the error
        //message over several lines
        $("#result_container").append("<h2>Error has occurred!</h2>");
        $("#result_container").append("<h5>The server has encountered a " + obj.status + " Error</h5>");
        $("#result_container").append("<p>" + obj.responseText + "</p>");


    };

    //Lastly, what are we going to do with our successful response
    var fnSuccess = function (ResponseData) {
        FIS.Example1.Log("Request Completed OK!");
        console.log(ResponseData);

        //due to a weird quirk in ASMX, the data that gets returned is always at "ResponseData.d", so we just grab that right away
        var data = ResponseData.d;

        //Now we've got our javascript object, if we look at it in the JS console we'll see something like:
        /*
        {
            Greeting:"Hello Todd"
            doubled_number:36
            today:"11/6/2017"
            tomorrow:"11/7/2017"
            yesterday:"11/5/2017"
            __type:"FIS2GoX1.Example_Code.Ex1.ComplexResponseObject"
        }
        */

        $("#result_container").html(""); //clear out the container

        $("#result_container").append("<h2>Request Completed OK!</h2>");
        $("#result_container").append("<h3>" + data.Greeting + "</h3>");
        $("#result_container").append("<table class='table' id='example_table'><thead><tr><th>Date</th><th>Temperature</th></tr></thead></table>");

        $("#example_table").append("<tr><td>" + data.yesterday + "</td><td>" + (data.doubled_number - 5) + "</td></tr>");
        $("#example_table").append("<tr><td>" + data.today + "</td><td>" + data.doubled_number  + "</td></tr>");
        $("#example_table").append("<tr><td>" + data.tomorrow + "</td><td>" + (data.doubled_number + 5) + "</td></tr>");

    };


    //Now, after setting everything up, we're ready to submit the AJAX request!
    // the request will return an object "var req = new $.ajax" but we'll never use it, the data will actually be
    // passed to the 3 callback functions we set up before. the AJAX call accepts only 1 parameter, which is an object with whatever
    // options we want to define
    // 
    // http://api.jquery.com/jquery.ajax/
    //
    var req = new $.ajax({                                  //as noted before, the $-denotes jQuery, this is the jQuery AJAX function
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        url: "./Ex1.asmx/Example1",
        data: strQueryParams,
        beforeSend: fnSent,
        success: fnSuccess,
        error: fnFailed
    });

    // Quick Glossary
    //  AJAX: A method of sending data to-from a web-browser to a web-server without a full page refresh
    //  JSON: The syntax for converting a Javascript Object to-from a string for saving or sending to server



   
};

//Extra Credit
// This last function is a VERY common jQuery function to delay the loading of code until the DOM is ready for manipulation
// this is different from Body.OnLoad which is more common in legacy environments. 
//      $(document): Our normal JQUERY selector, in this case we're 'selecting' the global-variable called 'document', we're not using a string selector
//      .ready: This is a function which subscribes to the "document" object's READY function
//      function () {: Create a function (callback, event handler, whatever) to run when the READY event completes

$(document).ready(function () {

    //   $("#ex1_btn").prop("disabled", false).addClass("btn-success"); : this is pretty similar to what we're doing above, find a button and set disabled=false and add a CSS class to turn it green
    //      This pattern, of selecting something and then using "." to run multiple commands against it is called "chaining" and is used to 
    //      increase readability and compactness
    $("#ex1_btn").prop("disabled", false).addClass("btn-success");
});



