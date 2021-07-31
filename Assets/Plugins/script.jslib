mergeInto(LibraryManager.library, {

  SaveLab: function (saveInput) {
    var saveStr = Pointer_stringify(saveInput);

    var msg = "Your lab has been saved in this link. You can reload a lab by going to this link or by clicking Load Lab and pasting this link into the prompt that will appear";
    if (saveStr.length > 2000) {
      if (confirm("Warning: Your lab is to big to save in a link and will be saved to a text file and will be downloaded to your computer. To load your lab, copy all of the text in the text file and paste it prompt from the Load Lab button. Click OK to download the file, or click cancel")) {
        var element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(saveStr));
        element.setAttribute('download', "save");

        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
      }
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
  


  DownloadPoints: function (csv) {
    var csvContent = "data:text/csv;charset=utf-8," + Pointer_stringify(csv);
    var encodedUri = encodeURI(csvContent);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "points.csv");
    document.body.appendChild(link); // Required for FF

    link.click(); 
    document.body.removeChild(link);
  },

  
});