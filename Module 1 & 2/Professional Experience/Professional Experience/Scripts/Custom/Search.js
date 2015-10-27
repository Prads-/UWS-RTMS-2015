function searchSel(textBox, dropDown) {
    var input = document.getElementById(textBox).value.toLowerCase();

    len = input.length;
    output = document.getElementById(dropDown).options;
    for (var i = 0; i < output.length; i++)
        if (output[i].text.toLowerCase().indexOf(input) != -1) {
            output[i].selected = true;
            break;
        }
    if (input == '')
        output[0].selected = true;
}