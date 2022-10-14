function confirmDelete(uniqueID, isClicked) {
    var deleteSpan = 'delete_' + uniqueID;
    var confirmDelete = 'confirmDelete_' + uniqueID;

    if (isClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDelete).show();
    }
    else {
        
        $('#' + deleteSpan).show();
        $('#' + confirmDelete).hide();
    }
}