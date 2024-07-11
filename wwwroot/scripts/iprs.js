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
    const handleTopId = `${id}-dots-container-top`;
    const handleBottomId = `${id}-dots-container-bottom`;
    const handleLeftId = `${id}-dots-container-left`;
    const handleRightId = `${id}-dots-container-right`;
    const panelId = `${id}-panel`;
    const stateChangerId = `${id}-panel-statechanger`;
    const panelcontainerId = `${id}-panel-container`;
    const handleLeft = document.getElementById(handleLeftId);
    const handleRight = document.getElementById(handleRightId);
    const handleTop = document.getElementById(handleTopId);
    const handleBottom = document.getElementById(handleBottomId);
    const panel = document.getElementById(panelId);
    const stateChanger = document.getElementById(stateChangerId);
    const panelcontainer = document.getElementById(panelcontainerId);

    window.panels[id] = {
        dotNetHelper: dotNetHelper,
        panelType: panelType,
        panelcontainer: panelcontainer
    };

    let startY;
    let startX;
    let initialWidth;
    let initialHeight;
    let initialStateChangerLeft;
    let initialStateChangerTop;

    if (handleLeft && handleRight)
    {
        if ((!handleLeft && !handleRight) || !panel || !stateChanger) return;


    }

    if (handleTop && handleBottom)
    {
        if ((!handleTop && !handleBottom) || !panel || !stateChanger) return;

    }

    const onMouseMove = (event) => {
        const diffX = event.clientX - startX;
        const diffY = event.clientY - startY;
        
        if (window.panels[id].panelType === 'Left') {
            panel.style.width = `${initialWidth + diffX}px`;
            stateChanger.style.setProperty('--state-changer-position', `${initialStateChangerLeft + diffX}px`);
        }
        else if (window.panels[id].panelType === 'Right')
        {
            panel.style.width = `${initialWidth - diffX}px`;
            panelcontainer.style.right = `${initialWidth - diffX}px`;
        }
        else if (window.panels[id].panelType === 'Top')
        {
            panel.style.height = `${initialHeight + diffY}px`;
            stateChanger.style.setProperty('--state-changer-position', `${initialStateChangerTop + diffY}px`);
        }
        else if (window.panels[id].panelType === 'Bottom') {
            panel.style.height = `${initialHeight - diffY}px`;
            panelcontainer.style.bottom = `${initialHeight - diffY}px`;
        }
    };

    const onMouseUp = () => {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        // Invoke the FinishedDragging method
        if (window.panels[id]) {
            var newSize; // Convert the size to a number
            if (handleTop && handleBottom)
            {
                newSize = parseFloat(panel.style.width);
                if (newSize < 100) {
                    panel.style.width = initialWidth + 'px';
                    if (window.panels[id].panelType === 'Left') {
                        stateChanger.style.setProperty('--state-changer-position', `${initialStateChangerLeft}px`);
                    }
                }
            }

            if (handleLeft && handleRight)
            {
                newSize = parseFloat(panel.style.height);
                if (newSize < 100) {
                    panel.style.height = initialHeight + 'px';
                    if (window.panels[id].panelType === 'Top') {
                        stateChanger.style.setProperty('--state-changer-position', `${initialStateChangerTop}px`);
                    }
                }
            }

            stateChanger.classList.remove('no-transition');
            panel.classList.remove('no-transition');
            panelcontainer.classList.remove('no-transition');



            window.panels[id].dotNetHelper.invokeMethodAsync('FinishedDragging', newSize).catch(err => console.error(err));
        }
    };

    const onMouseDown = (event) => {
        startY = event.clientY;
        startX = event.clientX;
        initialWidth = panel.offsetWidth;
        initialHeight = panel.offsetHeight;
        initialStateChangerTop = stateChanger.offsetTop;
        initialStateChangerLeft = stateChanger.offsetLeft;
        stateChanger.classList.add('no-transition');
        panel.classList.add('no-transition');
        panelcontainer.classList.add('no-transition');

        focusPanel(id, panelcontainer);

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    };

    const focusPanel = (focusedId, focusedPanelContainer) => {
        for (const id in window.panels) {
            const currentPanelContainer = window.panels[id].panelcontainer;
            if (currentPanelContainer) {
                if (id === focusedId) {
                    focusedPanelContainer.style.zIndex = 25; // Set this to the desired z-index value for focused panel
                }
                else
                {
                    if (window.panels[id].panelType === 'Right' || window.panels[id].panelType === 'Left') {
                        currentPanelContainer.style.zIndex = 21;
                    }
                    else
                    {
                        currentPanelContainer.style.zIndex = 22;
                    }
                }
            }
        }
    };

    if (handleLeft && handleRight)
    {
        handleLeft.addEventListener('mousedown', onMouseDown);
        handleRight.addEventListener('mousedown', onMouseDown);
        panel.addEventListener('mousedown', () => focusPanel(id, panelcontainer));
    }

    if (handleTop && handleBottom)
    {
        handleTop.addEventListener('mousedown', onMouseDown);
        handleBottom.addEventListener('mousedown', onMouseDown);
        panel.addEventListener('mousedown', () => focusPanel(id, panelcontainer));
    }
};



window.tables = {};

window.registerTables = function (id, dotNetHelper) {

    const tableId = `${id}-table`;
    const table = document.getElementById(tableId);

    window.tables[tableId] = {
        dotNetHelper: dotNetHelper
    };

    if (!table) {
        console.error(`Table with ID '${tableId}' not found.`);
        return;
    } else {
        console.log(`Table with ID '${tableId}' found.`);
    }

    const headers = table.querySelectorAll("th");

    headers.forEach((header) => {
        const resizer = document.createElement("div");
        resizer.className = "resizer";
        resizer.addEventListener("mousedown", (e) => startDrag(e, header, tableId));
        header.appendChild(resizer);
    });

    function startDrag(e, header, tableId) {
        const onDrag = (e) => onDragHandler(e, header);
        const stopDrag = () => stopDragHandler(onDrag, stopDrag);

        document.addEventListener("mousemove", onDrag);
        document.addEventListener("mouseup", stopDrag);

        let startX = e.clientX;
        let startWidth = header.offsetWidth;

        function onDragHandler(e) {
            let newWidth = startWidth + (e.clientX - startX);
            header.style.width = `${newWidth}px`;
        }

        function stopDragHandler(onDrag, stopDrag) {
            document.removeEventListener("mousemove", onDrag);
            document.removeEventListener("mouseup", stopDrag);
            // Invoke a method on the Blazor component (if needed)
            window.tables[tableId].dotNetHelper.invokeMethodAsync('OnColumnResized', header.innerText, newWidth);
        }
    }
};
