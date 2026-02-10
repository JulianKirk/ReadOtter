window.clickElement = (element) => {
    element.click();
};

window.addEventListener('keydown', (event) => {
    DotNet.invokeMethodAsync('ReadOtter.Shared', 'OnKeyDown', event.key);
});

window.addEventListener('keydown', (event) => {
    DotNet.invokeMethodAsync('ReadOtter', 'OnKeyDown', event.key);
});
