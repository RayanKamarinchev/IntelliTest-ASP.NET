var mathFields = document.querySelectorAll("#questions > div > div > div > textarea");
var latexSpan = document.getElementById('latex');

var MQ = MathQuill.getInterface(2); // for backcompat
[...mathFields].forEach(f => {
    MQ.MathField(f, {
        spaceBehavesLikeTab: true, 
        handlers: {
            edit: function () { // useful event handlers
                latexSpan.textContent = mathField.latex(); // simple API
            }
        }
    });
})