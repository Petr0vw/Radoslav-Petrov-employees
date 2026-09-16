document.addEventListener("DOMContentLoaded", function () {
    const fileInput = document.getElementById("csvFile");
    const fileName = document.getElementById("selectedFileName");

    if (!fileInput || !fileName) {
        return;
    }

    fileInput.addEventListener("change", function () {
        if (fileInput.files.length > 0) {
            fileName.textContent = fileInput.files[0].name;
        } else {
            fileName.textContent = "No file selected";
        }
    });
}); 