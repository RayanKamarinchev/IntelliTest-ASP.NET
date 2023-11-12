function fileDragHover(e) {
  let fileDrag = e.target
  e.preventDefault();
  fileDrag.classList[1] = (e.type === 'dragover' ? 'hover' : 'modal-body file-upload');
}

function fileSelectHandler(e) {
  // Fetch FileList object
  var files = e.target.files || e.dataTransfer.files;

  // Cancel event and hover styling
  fileDragHover(e);

  // Process all File objects
  for (var i = 0, f; f = files[i]; i++) {
    parseFile(f, e.target);
  }
}

function parseFile(file, target) {
  let imageName = file.name;

  let isGood = (/\.(?=gif|jpg|png|jpeg)/gi).test(imageName);
  let imageUploaderIndex =[...document.getElementsByClassName("file-upload")].indexOf(target);
  if (isGood) {
    document.getElementsByClassName('start')[imageUploaderIndex].classList.add("hidden");
    document.getElementsByClassName('response')[imageUploaderIndex].classList.remove("hidden");
    document.getElementsByClassName('notimage')[imageUploaderIndex].classList.add("hidden");
    // Thumbnail Preview
    document.getElementsByClassName('file-image')[imageUploaderIndex].classList.remove("hidden");
    document.getElementsByClassName('file-image')[imageUploaderIndex].src = URL.createObjectURL(file);
  }
  else {
    document.getElementsByClassName('file-image')[imageUploaderIndex].classList.add("hidden");
    document.getElementsByClassName('notimage')[imageUploaderIndex].classList.remove("hidden");
    document.getElementsByClassName('start')[imageUploaderIndex].classList.remove("hidden");
    document.getElementsByClassName('response')[imageUploaderIndex].classList.add("hidden");
    document.getElementsByClassName("file-upload-form")[imageUploaderIndex].reset();
  }
}

function setUpFileUpload(){
    let fileSelects= [...document.getElementsByClassName('file-upload')]
    let fileDrags      = [...document.getElementsByClassName('file-drag')];
        fileSelects.forEach(x => x.addEventListener('change', fileSelectHandler, false))

    fileDrags.forEach(fileDrag => {
      fileDrag.addEventListener('dragover', fileDragHover, false);
      fileDrag.addEventListener('dragleave', fileDragHover, false);
      fileDrag.addEventListener('drop', fileSelectHandler, false);
    })
}

setUpFileUpload();

function showImageUpload(e) {
  if (e.type === "click"){
    e = e.target;
  }
  let element = e.parentElement.children[0]
  console.log(element)
  if (element.style.display === "none")
    element.style.display = "block"
  else
    element.style.display = "none"
}

let btns = [...document.getElementsByClassName("addImageToQuestionBtn")];
for (const btn of btns) {
  btn.addEventListener("click", showImageUpload)
}