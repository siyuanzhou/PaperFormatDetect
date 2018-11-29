function alertWarning(msg) {
  msg = "<h3 style='text-align:center;'>" + msg + "</h3>";
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_DANGER,
    cssClass: 'set-dialog',
    title: "消息提示",
    message: msg,
    buttons: [{
      label: '关闭',
      action: function(dialogRef) {
        dialogRef.close();
      }
    }]
  });
}

function alertInfo(msg) {
  msg = "<h3 style='text-align:center;'>" + msg + "</h3>";
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_INFO,
    cssClass: 'set-dialog',
    title: "消息提示",
    message: msg,
    buttons: [{
      label: '关闭',
      action: function(dialogRef) {
        dialogRef.close();
      }
    }]
  });
}