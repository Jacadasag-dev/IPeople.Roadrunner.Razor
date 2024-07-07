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



window.panels = {};

window.registerPanels = (id, dotNetHelper, panelType) => {
    window.panels[id] = {
        dotNetHelper: dotNetHelper,
        panelType: panelType
    };
    console.log(`Panel registered: ${id}, Type: ${panelType}`, dotNetHelper);

    const handleId = `${id}-dots-container-top`;
    const panelId = `${id}-panel`;
    const stateChangerId = `${id}-panel-statechanger`;
    const panelcontainerId = `${id}-panel-container`;
    const handle = document.getElementById(handleId);
    const panel = document.getElementById(panelId);
    const stateChanger = document.getElementById(stateChangerId);
    const panelcontainer = document.getElementById(panelcontainerId);

    if (!handle || !panel || !stateChanger) return;

    let startX;
    let initialWidth;
    let initialStateChangerLeft;
    let initialPositionRight;

    const onMouseMove = (event) => {
        const diffX = event.clientX - startX;
        
        if (window.panels[id].panelType === 'Left') {
            panel.style.width = `${initialWidth + diffX}px`;
            stateChanger.style.setProperty('--state-changer-position', `${initialStateChangerLeft + diffX}px`);
        }
        else if (window.panels[id].panelType === 'Right')
        {
            panel.style.width = `${initialWidth - diffX}px`;
            panelcontainer.style.right = `${initialPositionRight - diffX}px`;
        }
    };

    const onMouseUp = () => {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        stateChanger.classList.remove('no-transition');

        if (window.panels[id].panelType === 'Right')
        {
            panelcontainer.classList.remove('no-transition');
        }

        // Invoke the FinishedDragging method
        if (window.panels[id]) {
            const newSize = parseFloat(panel.style.width); // Convert the size to a number
            window.panels[id].dotNetHelper.invokeMethodAsync('FinishedDragging', newSize)
                .then(() => console.log(`Finished dragging for panel: ${id}, new size: ${newSize}, type: ${window.panels[id].panelType}`))
                .catch(err => console.error(err));
        }
    };

    const onMouseDown = (event) => {
        startX = event.clientX;
        initialWidth = panel.offsetWidth;
        initialPosition = panel.offsetLeft;
        initialPositionRight = window.innerWidth - panelcontainer.offsetLeft;
        initialStateChangerLeft = stateChanger.offsetLeft;
        initialStateChangerRight = 0;
        stateChanger.classList.add('no-transition');
        if (window.panels[id].panelType === 'Right')
        {
            panelcontainer.classList.add('no-transition');
        }

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    };

    handle.addEventListener('mousedown', onMouseDown);
};
