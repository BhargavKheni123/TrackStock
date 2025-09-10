function checkpass(txtpasID) {
    var str = $("#" + txtpasID).val();
    var re = '^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$';
    var found = str.match(re);
    if (found == null) {
        return false;
    }
    return true;
}