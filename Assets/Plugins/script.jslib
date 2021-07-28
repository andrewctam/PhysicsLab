mergeInto(LibraryManager.library, {

  SaveLab: function (saveInput) {
    var saveStr = Pointer_stringify(saveInput);

    var msg = "Your lab has been saved in this link. You can reload a lab by going to this link or by clicking Load Lab and pasting this link into the prompt that will appear";
    if (saveStr.length > 2000) {
      alert("Warning: Your lab will be saved but can not be accessed directly with a link since the link is greater than 2000 characters, which is the limit for most web browsers. To load your lab, use the Load Lab Button.");

      document.getElementById("save").value = saveStr;
      document.getElementById("save").style.display = "block";
      
    }
    else
      prompt(msg, saveStr);
  },

  LoadNewLab: function () {
    var msg = "Enter a link below to load a lab.";
    var loadOutput = prompt(msg, "");

    if (loadOutput != null && confirm("The current lab will be overwritten and deleted. Would you like to load a new lab?"))  {
      var i = loadOutput.indexOf('?');
      if (i != -1) {
        loadOutput = loadOutput.substring(i + 1);
      }
      SendMessage('Canvas', 'loadLab', loadOutput);
    }
    
  },
  
  CheckForLoad: function () {
    var url = window.location.href;
    var i = url.indexOf('?');
    if (i != -1) {
      url = url.substring(i + 1);
      SendMessage('Canvas', 'loadLab', url);
    }

  },
});