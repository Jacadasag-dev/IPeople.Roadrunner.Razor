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

class Panel {
    constructor(id, panelElement, dotNetHelper, type, latching, container, stateChanger, minLatchingWidth) {
        this.id = id;
        this.element = panelElement;
        this.initialWidth = 0;
        this.initialHeight = 0;
        this.container = container;
        this.stateChanger = stateChanger;
        this.initialStateChangerLeft = "";
        this.initialStateChangerTop = "";
        this.type = type;
        this.latching = latching;
        this.dotNetHelper = dotNetHelper;
        this.minLatchingWidth = minLatchingWidth
    }

    disableTransitions() {
        this.stateChanger.classList.add('no-transition');
        this.element.classList.add('no-transition');
        this.container.classList.add('no-transition');
    }

    enableTransitions() {
        this.stateChanger.classList.remove('no-transition');
        this.element.classList.remove('no-transition');
        this.container.classList.remove('no-transition');
    }
}

window.panels = {};

window.registerPanels = (id, dotNetHelper, panelType, latching, minLatchingWidth) => {
    const panelElement = document.getElementById(`${id}-panel`);
    if (!panelElement) return;

    if (!window.panels[id]) {
        const container = document.getElementById(`${id}-panel-container`);
        const stateChanger = document.getElementById(`${id}-panel-statechanger`);
        window.panels[id] = new Panel(id, panelElement, dotNetHelper, panelType, latching, container, stateChanger, minLatchingWidth);
    }
    const handleLeft = document.getElementById(`${id}-dots-container-left`);
    const handleRight = document.getElementById(`${id}-dots-container-right`);
    const handleTop = document.getElementById(`${id}-dots-container-top`);
    const handleBottom = document.getElementById(`${id}-dots-container-bottom`);
    let leftPanel = null;
    let rightPanel = null;
    let startY;
    let startX;

    if (window.panels[id].latching) {
        for (const key in window.panels) {
            let panel = window.panels[key];
            if (panel.type === 'Left') leftPanel = panel;
            if (panel.type === 'Right') rightPanel = panel;
        }
    }

    const onMouseMove = (event) => {
        const diffX = event.clientX - startX;
        const diffY = event.clientY - startY;
        
        if (window.panels[id].latching && leftPanel && rightPanel) {
            const minWidth = leftPanel.minLatchingWidth;
            const newLeftWidth = leftPanel.initialWidth + diffX;
            const newRightWidth = rightPanel.initialWidth - diffX;

            if (newLeftWidth >= minWidth && newRightWidth >= minWidth) {
                leftPanel.element.style.width = `${newLeftWidth}px`;
                leftPanel.stateChanger.style.setProperty('--state-changer-position', `${leftPanel.initialStateChangerLeft + diffX}px`);
                rightPanel.element.style.width = `${newRightWidth}px`;
                rightPanel.container.style.right = `${newRightWidth}px`;
            }
        } else {
            const panel = window.panels[id];
            if (panel.type === 'Left') {
                panel.element.style.width = `${panel.initialWidth + diffX}px`;
                panel.stateChanger.style.setProperty('--state-changer-position', `${panel.initialStateChangerLeft + diffX}px`);
            } else if (panel.type === 'Right') {
                panel.element.style.width = `${panel.initialWidth - diffX}px`;
                panel.container.style.right = `${panel.initialWidth - diffX}px`;
            } else if (panel.type === 'Top') {
                panel.element.style.height = `${panel.initialHeight + diffY}px`;
                panel.stateChanger.style.setProperty('--state-changer-position', `${panel.initialStateChangerTop + diffY}px`);
            } else if (panel.type === 'Bottom') {
                panel.element.style.height = `${panel.initialHeight - diffY}px`;
                panel.container.style.bottom = `${panel.initialHeight - diffY}px`;
            }
        }
    };

    const onMouseUp = () => {
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);

        const panel = window.panels[id];
        let newSize = -1;

        let rightPanelId;
        let leftSize = -1;
        let leftPanelId;

        if (panel.latching && leftPanel && rightPanel) {
            leftPanelId = leftPanel.id;
            leftSize = parseFloat(leftPanel.element.style.width);
            if (leftSize < 100) {
                leftPanel.element.style.width = `${leftPanel.initialWidth}px`;
                leftPanel.stateChanger.style.setProperty('--state-changer-position', `${leftPanel.initialStateChangerLeft}px`);
            }
            rightSize = parseFloat(rightPanel.element.style.width);
            rightPanelId = rightPanel.id;
            if (rightSize < 100) {
                rightPanel.element.style.width = `${rightPanel.initialWidth}px`;
            }
            leftPanel.enableTransitions();
            rightPanel.enableTransitions();
            newSize = leftSize;
        } else {
            if (handleTop && handleBottom) {
                newSize = parseFloat(panel.element.style.width);
                if (newSize < 100) {
                    panel.element.style.width = `${panel.initialWidth}px`;
                    if (panel.type === 'Left') {
                        panel.stateChanger.style.setProperty('--state-changer-position', `${panel.initialStateChangerLeft}px`);
                    }
                }
            }
            if (handleLeft && handleRight) {
                newSize = parseFloat(panel.element.style.height);
                if (newSize < 100) {
                    panel.element.style.height = `${panel.initialHeight}px`;
                    if (panel.type === 'Top') {
                        panel.stateChanger.style.setProperty('--state-changer-position', `${panel.initialStateChangerTop}px`);
                    }
                }
            }
            panel.enableTransitions();
        }

        panel.dotNetHelper.invokeMethodAsync('FinishedDragging', newSize, leftPanelId, rightPanelId).catch(err => console.error(err));
    };

    function initializePanelState(panel) {
        if (!panel || !panel.stateChanger || !panel.element) {
            console.error('Invalid panel object');
            return;
        }

        panel.initialStateChangerLeft = panel.stateChanger.offsetLeft;
        panel.initialStateChangerTop = panel.stateChanger.offsetTop;
        panel.initialWidth = panel.element.offsetWidth;
        panel.initialHeight = panel.element.offsetHeight;
    }

    const onMouseDown = (event) => {
        startY = event.clientY;
        startX = event.clientX;
        const panel = window.panels[id];

        if (panel.latching && leftPanel && rightPanel) {
            leftPanel.disableTransitions();
            rightPanel.disableTransitions();
            initializePanelState(leftPanel);
            initializePanelState(rightPanel);
        } else {
            panel.disableTransitions();
            initializePanelState(panel);
        }

        focusPanel();

        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    };

    const focusPanel = () => {
        for (const key in window.panels) {
            const panel = window.panels[key];
            let action = "";
            if (window.panels[id].latching) {
                action = "latching-collapse";
            }
            else
            {
                action = "panel-focused";
            }
            panel.dotNetHelper.invokeMethodAsync('PanelClickedOnScriptHandler', id, action).catch(err => console.error(err));
        }
    };

    if (window.panels[id].latching && leftPanel && rightPanel) {
        leftPanel.stateChanger.addEventListener('mousedown', onMouseDown);
        rightPanel.stateChanger.addEventListener('mousedown', onMouseDown);
        leftPanel.element.addEventListener('mousedown', () => focusPanel(leftPanel));
        rightPanel.element.addEventListener('mousedown', () => focusPanel(rightPanel));
    }
    else {
        if (handleLeft && handleRight) {
            handleLeft.addEventListener('mousedown', onMouseDown);
            handleRight.addEventListener('mousedown', onMouseDown);
            window.panels[id].element.addEventListener('mousedown', () => focusPanel(window.panels[id]));
            window.panels[id].stateChanger.addEventListener('mousedown', () => focusPanel(window.panels[id]));
        }
        if (handleTop && handleBottom) {
            handleTop.addEventListener('mousedown', onMouseDown);
            handleBottom.addEventListener('mousedown', onMouseDown);
            window.panels[id].element.addEventListener('mousedown', () => focusPanel(window.panels[id]));
            window.panels[id].stateChanger.addEventListener('mousedown', () => focusPanel(window.panels[id]));
        }
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


