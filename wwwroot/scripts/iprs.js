window.dropdownInstances = {};

window.registerDropdown = function (id, dotNetObjectRef) {
    window.dropdownInstances[id] = dotNetObjectRef;
};

window.invokeHandleDropdownClicked = function (excludeId = null) {
    for (const id in window.dropdownInstances) {
        if (id !== excludeId) {
            window.dropdownInstances[id].invokeMethodAsync('CollapseDropdownIfOpen');
        }
    }
};

document.addEventListener('click', function (event) {
    let targetElement = event.target;
    while (targetElement != null) {
        if (targetElement.classList && targetElement.classList.contains('Rr-dropdown-container')) {
            return;
        }
        targetElement = targetElement.parentElement;
    }
    window.invokeHandleDropdownClicked();
});

function observeElementWidth(elementId, narrowClassName, longClassName, narrowThresholdWidth, longThreshholdWidth) {
    const element = document.getElementById(elementId);

    if (!element) {
        console.error(`Element with ID ${elementId} not found.`);
        return;
    }

    function checkWidth() {
        if (element.offsetWidth <= narrowThresholdWidth) {
            console.log(`${elementId}: add narrow class.`);
            element.classList.add(narrowClassName);
            element.classList.remove(longClassName);
        } else if (element.offsetWidth >= longThreshholdWidth) {
            console.log(`${elementId}: add long class.`);
            element.classList.add(longClassName);
            element.classList.remove(narrowClassName);
        } else {
            console.log(`${elementId}: remove both classes.`);
            element.classList.remove(narrowClassName);
            element.classList.remove(longClassName);
        }
    }

    const resizeObserver = new ResizeObserver(checkWidth);
    resizeObserver.observe(element);

    // Initial check
    checkWidth();
}

// Expose the function to be callable from Blazor
window.observeElementWidth = observeElementWidth;


window.makeDraggable = function (id) {
    const draggableElement = document.getElementById(id);

    if (!draggableElement) {
        return;
    }

    let pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;

    if (document.getElementById(id + "-header")) {
        document.getElementById(id + "-header").onmousedown = dragMouseDown;
    } else {
        draggableElement.onmousedown = dragMouseDown;
    }

    function dragMouseDown(e) {
        e = e || window.event;
        e.preventDefault();
        pos3 = e.clientX;
        pos4 = e.clientY;
        document.onmouseup = closeDragElement;
        document.onmousemove = elementDrag;
    }

    function elementDrag(e) {
        e = e || window.event;
        e.preventDefault();
        pos1 = pos3 - e.clientX;
        pos2 = pos4 - e.clientY;
        pos3 = e.clientX;
        pos4 = e.clientY;
        draggableElement.style.top = (draggableElement.offsetTop - pos2) + "px";
        draggableElement.style.left = (draggableElement.offsetLeft - pos1) + "px";
    }

    function closeDragElement() {
        document.onmouseup = null;
        document.onmousemove = null;
    }
};

function getElementBounds(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        var rect = element.getBoundingClientRect();
        return {
            top: rect.top,
            left: rect.left,
            width: rect.width,
            height: rect.height,
            right: rect.right,
            bottom: rect.bottom
        };
    }
    return null;
}

window.setupResizeListener = function (elementId, dotNetHelper) {
    var element = document.getElementById(elementId);
    if (element) {
        var updateBounds = function () {
            var bounds = window.getElementBounds(elementId);
            dotNetHelper.invokeMethodAsync('UpdateBounds', bounds);
        };
        window.addEventListener('resize', updateBounds);
        if (element) {
            new ResizeObserver(updateBounds).observe(element);
        }
    }
};




