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
class RrPage {
    constructor(id, bounds, panels) {
        this.id = id;
        this.bounds = bounds;
        this.panels = panels;
    }
}
class RrPanel {
    constructor(id, panelElement, dotNetHelper, type, latchingType, container, stateChanger, minLatchingWidth, latching, size, state, dots1, dots2, stateToggler, latchingStateToggler) {
        this.id = id;
        this.element = panelElement;
        this.container = container;
        this.stateChanger = stateChanger;
        this.type = type;
        this.latchingType = latchingType;
        this.dotNetHelper = dotNetHelper;
        this.minLatchingWidth = minLatchingWidth;
        this.latching = latching;
        this.size = size;
        this.state = state;
        this.dots1 = dots1;
        this.dots2 = dots2;
        this.arrow = stateToggler;
        this.centerDots = latchingStateToggler;
        this.isFocused = false;
        this.wasFocused1 = false;
        this.wasFocused2 = false;
        this.wasFocused3 = false;
        this.clickOrder = 0;
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

    makeExpanded() {
        if (this.stateChanger.classList.contains('minimized')) this.stateChanger.classList.remove('minimized');
        if (this.container.classList.contains('minimized')) this.container.classList.remove('minimized');
        if (this.element.classList.contains('minimized')) this.element.classList.remove('minimized');
        this.stateChanger.classList.add('expanded');
        this.container.classList.add('expanded');
        this.element.classList.add('expanded');
    }

    makeMinimized() {
        if (this.stateChanger.classList.contains('expanded')) this.stateChanger.classList.remove('expanded');
        if (this.container.classList.contains('expanded')) this.container.classList.remove('expanded');
        if (this.element.classList.contains('expanded')) this.element.classList.remove('expanded');
        this.stateChanger.classList.add('minimized');
        this.container.classList.add('minimized');
        this.element.classList.add('minimized');
    }
}

window.RrPage = {};

window.registerPageAndPanels = function (pageId, panelDtos) {
    // Register Page
    if (!window.RrPage[pageId]) {
        window.RrPage[pageId] = new RrPage(pageId, null, []);
        window.RrPage[pageId].bounds = getPageElementBounds(pageId);
    }

    // Register Panels
    var panels = [];
    panelDtos.forEach(function (dto) {
        const panelElement = document.getElementById(`${dto.id}-panel`);
        const container = document.getElementById(`${dto.id}-panel-container`);
        const stateChanger = document.getElementById(`${dto.id}-panel-statechanger`);
        const stateToggler = document.getElementById(`${dto.id}-panel-arrow-container`);
        const latchingStateToggler = document.getElementById(`${dto.id}-panel-dots-container-center`);
        let dots1;
        let dots2;
        if (dto.pType === 'Left' || dto.pType === 'Right') {
            dots1 = document.getElementById(`${dto.id}-dots-container-top`);
            dots2 = document.getElementById(`${dto.id}-dots-container-bottom`);
        } else if (dto.pType === 'Top' || dto.pType === 'Bottom') {
            dots1 = document.getElementById(`${dto.id}-dots-container-left`);
            dots2 = document.getElementById(`${dto.id}-dots-container-right`);
        }
        const panel = new RrPanel(dto.id, panelElement, dto.dotNetObjectReference, dto.pType, dto.latchingType, container, stateChanger, dto.minLatchingWidth, dto.latching, dto.size, dto.state, dots1, dots2, stateToggler, latchingStateToggler);
        panels.push(panel);

        const onMouseDown = (event) => {
            let startY = event.clientY;
            let startX = event.clientX;
            const initialHieght = panel.element.offsetHeight;
            const initialWidth = panel.element.offsetWidth
            const initialStateChangerLeft = panel.stateChanger.offsetLeft;
            const initialStateChangerTop = panel.stateChanger.offsetTop;
            const leftPanelInitialWidth = window.RrPage[pageId].panels.find(p => p.type === 'Left').element.offsetWidth
            if (panel.latching) {
                if (panel.latchingType === 'Vertical') {
                    leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                    rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
                    if (leftPanel && rightPanel) {
                        leftPanel.disableTransitions();
                        rightPanel.disableTransitions();
                    }
                }
            }
            panel.disableTransitions();

            const onMouseMove = (event) => {
                const diffX = event.clientX - startX;
                const diffY = event.clientY - startY;
                if (panel.latching) {
                    if (panel.latchingType === 'Vertical') {
                        leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                        rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
                        if (leftPanel && rightPanel) {
                            // Calculate the new width while respecting the minimum latching width
                            let newLeftPanelWidth = leftPanelInitialWidth + diffX;
                            let newRightPanelWidth = window.RrPage[pageId].bounds.width - 20 - newLeftPanelWidth;

                            if (newLeftPanelWidth < leftPanel.minLatchingWidth) {
                                newLeftPanelWidth = leftPanel.minLatchingWidth;
                                newRightPanelWidth = window.RrPage[pageId].bounds.width - 20 - newLeftPanelWidth;
                            }

                            if (newRightPanelWidth < rightPanel.minLatchingWidth) {
                                newRightPanelWidth = rightPanel.minLatchingWidth;
                                newLeftPanelWidth = window.RrPage[pageId].bounds.width - 20 - newRightPanelWidth;
                            }

                            // Apply the new widths
                            leftPanel.element.style.width = `${newLeftPanelWidth}px`;
                            leftPanel.stateChanger.style.left = `${newLeftPanelWidth}px`;
                            rightPanel.container.style.right = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${newLeftPanelWidth}px)`;
                            rightPanel.element.style.width = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${newLeftPanelWidth}px)`;
                        }
                    }
                } else {
                    if (panel.type === 'Left') {
                        panel.element.style.width = `${initialWidth + diffX}px`;
                        panel.stateChanger.style.left = `${initialStateChangerLeft + diffX}px`;
                    } else if (panel.type === 'Right') {
                        panel.element.style.width = `${initialWidth - diffX}px`;
                        panel.container.style.right = `${initialWidth - diffX}px`;
                    } else if (panel.type === 'Top') {
                        panel.element.style.height = `${initialHieght + diffY}px`;
                        panel.stateChanger.style.top = `${initialStateChangerTop + diffY}px`;
                    } else if (panel.type === 'Bottom') {
                        panel.element.style.height = `${initialHieght - diffY}px`;
                        panel.container.style.bottom = `${initialHieght - diffY}px`;
                    }
                }
            };

            const onMouseUp = () => {
                document.removeEventListener('mousemove', onMouseMove);
                document.removeEventListener('mouseup', onMouseUp);

                
                if (panel.latching) {
                    if (panel.latchingType === 'Vertical') {
                        let leftSize = -1;
                        let rightSize = -1;
                        leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                        rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
                        if (leftPanel && rightPanel) {
                            leftSize = parseFloat(leftPanel.element.style.width);
                            rightSize = parseFloat(rightPanel.element.style.width);
                            panel.enableTransitions();
                            leftPanel.size = `${leftSize}px`;
                            rightPanel.size = `${rightSize}px`;
                        }
                    }
                } else {
                    let size = -1;
                    if (panel.type === 'Left' || panel.type === 'Right') {
                        size = parseFloat(panel.element.style.width);
                        if (size < 100) {
                            toggleUIState(panel);
                            if (panel.type === 'Left') {
                                panel.stateChanger.style.left = panel.size;
                                setTimeout(function () {
                                    panel.element.style.width = panel.size;
                                }, 200);
                                
                            } else {
                                panel.element.style.width = panel.size;
                            }
                        }
                    } else if (panel.type === 'Top' || panel.type === 'Bottom') {
                        size = parseFloat(panel.element.style.height);
                        if (size < 100) {
                            toggleUIState(panel);
                            if (panel.type === 'Top') {
                                panel.stateChanger.style.top = panel.size;
                                setTimeout(function () {
                                    panel.element.style.height = panel.size;
                                }, 200);
                            } else {
                                panel.element.style.height = panel.size;
                            }    
                        }
                    }
                    panel.enableTransitions();
                    if (size >= 100) {
                        panel.size = `${size}px`;
                    }
                }
            };

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        };

        if (!panel.latching) {
            panel.arrow.addEventListener('mousedown', () => toggleUIState(panel));
            panel.dots1.addEventListener('mousedown', onMouseDown);
            panel.dots2.addEventListener('mousedown', onMouseDown);
        } else {
            panel.stateChanger.addEventListener('mousedown', onMouseDown);
        }
        panel.element.addEventListener('mousedown', () => focusPanel(panel));
        panel.stateChanger.addEventListener('mousedown', () => focusPanel(panel));
        
    });
    window.RrPage[pageId].panels = panels;

    window.setPanelUIState = function (myPageId, panelId, desiredState) {
        const panel = window.RrPage[myPageId].panels.find(p => p.id === panelId);
        if (panel) {
            console.log(`Setting ${panel.type} state to ${desiredState}`);
            toggleUIState(panel, desiredState);
        }
    };

    function getPageElementBounds(elementId) {
        const element = document.getElementById(elementId);
        if (!element) return null;
        const rect = element.getBoundingClientRect();
        return {
            top: rect.top + window.scrollY,
            left: rect.left + window.scrollX,
            right: rect.right + window.scrollX,
            bottom: rect.bottom + window.scrollY,
            width: rect.width,
            height: rect.height,
        };
    }
    function getVerticalPanelHeightOffsets(panel) {
        if (panel.latching)
            return -20;

        var offset = 0;
        var bottomPanel = window.RrPage[pageId].panels.find(p => p.type === 'Bottom');
        var topPanel = window.RrPage[pageId].panels.find(p => p.type === 'Top');
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Expanded') 
            offset = 0;

        if (bottomPanel && bottomPanel.state === 'Collapsed' && topPanel && topPanel.state === 'Collapsed') 
            offset = -20;
        
        if (bottomPanel && bottomPanel.state === 'Collapsed' && topPanel && topPanel.state === 'Expanded') 
            offset = -10;
        
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Collapsed') 
            offset = -10;

        return offset;
    }
    function getHorizontalPanelWidthOffsets(panel) {
        var offset = 0;
        if (panel && panel.state === 'Expanded') {
            offset = 0;
        }
        if (panel && panel.state === 'Collapsed') {
            offset = 0;
        }

        return offset;
    }
    function getVerticalPanelTopOffsets(panel) {
        if (panel.latching) 
            return 10;
        
        var offset = 0;
        var bottomPanel = window.RrPage[pageId].panels.find(p => p.type === 'Bottom');
        var topPanel = window.RrPage[pageId].panels.find(p => p.type === 'Top');
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Expanded') 
            offset = 0;
        
        if (bottomPanel && bottomPanel.state === 'Collapsed' && topPanel && topPanel.state === 'Collapsed') 
            offset = 10;
        
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Collapsed') 
            offset = 10;
        
        return offset;
    }
    function getHorizontalPanelLeftOffsets(panel) {
        var offset = 0;
        if (panel && panel.state === 'Expanded') 
            offset = 0;
        
        if (panel && panel.state === 'Collapsed') 
            offset = 0;
        
        return offset;
    }
    function focusPanel(panel) {
        const panels = window.RrPage[pageId].panels;
        if (!panel.latching) {
            panels.forEach(p => {
                p.isFocused = (p.id === panel.id);
            });

            panels.sort((a, b) => {
                if (a.isFocused && !b.isFocused) return -1;
                if (!a.isFocused && b.isFocused) return 1;
                return 0;
            });

            panels.forEach((p, i) => {
                p.container.style.zIndex = `${20 + (panels.length - i)}`;
            });

            

            panels.forEach((p, i) => {
                setPanelBounds(p);
            });

            if (panel.state === 'Collapsed') {
                panel.container.style.zIndex = 20;
            }
        }
    };
    function toggleUIState(panel, desiredState) {
        const stateToSet = desiredState || (panel.state === 'Collapsed' ? 'Expanded' : 'Collapsed');
        if (panel.latching)
            return;

        if (panel.type === 'Left') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.left = `0px`;
                panel.dots1.style.display = 'block';
                panel.dots2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.left = `-${panel.size}`;
                panel.dots1.style.display = 'none';
                panel.dots2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Right') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.right = `${panel.size}`;
                panel.dots1.style.display = 'block';
                panel.dots2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.right = `0px`;
                panel.dots1.style.display = 'none';
                panel.dots2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Top') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.top = `0px`;
                panel.dots1.style.display = 'block';
                panel.dots2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.top = `-${panel.size}`;
                panel.dots1.style.display = 'none';
                panel.dots2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Bottom') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.bottom = `${panel.size}`;
                panel.dots1.style.display = 'block';
                panel.dots2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.bottom = `0px`;
                panel.dots1.style.display = 'none';
                panel.dots2.style.display = 'none';
                panel.makeMinimized();
            }
        }
        panel.dotNetHelper.invokeMethodAsync('UpdateStateServicePanelState', panel.state).catch(err => console.error(err));
    }
    function setPanelBounds(panel) {
        if (panel.latching) {
            if (panel.latchingType === 'Vertical') {
                leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
                if (leftPanel && rightPanel) {
                    panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                    panel.stateChanger.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.centerDots.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.element.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.stateChanger.style.width = "10px";
                    leftPanel.container.style.left = "0px";
                    leftPanel.element.style.width = panel.size;
                    leftPanel.stateChanger.style.left = panel.size;
                    rightPanel.container.style.right = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${panel.size})`;
                    rightPanel.element.style.width = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${panel.size})`;
                    rightPanel.stateChanger.style.right = `0px`;
                    leftPanel.container.style.zIndex = 19;
                    rightPanel.container.style.zIndex = 18;
                }
            }
        } else {
            if (panel.type === 'Top') {
                if (panel.state === 'Expanded')
                    panel.container.style.top = `0px`;
                else
                    panel.container.style.top = `-${panel.size}`;

                panel.container.style.left = `${window.RrPage[pageId].bounds.left + getHorizontalPanelLeftOffsets(panel)}px`;
                panel.element.style.width = `${window.RrPage[pageId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.element.style.height = panel.size;
                panel.stateChanger.style.width = `${window.RrPage[pageId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.stateChanger.style.height = "10px";
                panel.stateChanger.style.top = panel.size;
            } else if (panel.type === 'Bottom') {
                if (panel.state === 'Expanded')
                    panel.container.style.bottom = panel.size;
                else
                    panel.container.style.bottom = `0px`;

                panel.container.style.left = `${window.RrPage[pageId].bounds.left + getHorizontalPanelLeftOffsets(panel)}px`;
                panel.element.style.width = `${window.RrPage[pageId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.element.style.height = panel.size;
                panel.stateChanger.style.width = `${window.RrPage[pageId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.stateChanger.style.height = "10px";
                panel.stateChanger.style.bottom = "0px";
            } else if (panel.type === 'Left') {
                if (panel.state === 'Expanded')
                    panel.container.style.left = "0px";
                else
                    panel.container.style.left = `-${panel.size}`;

                panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                panel.element.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.element.style.width = panel.size;
                panel.stateChanger.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.stateChanger.style.width = "10px";
                panel.stateChanger.style.left = panel.size;
            } else if (panel.type === 'Right') {
                if (panel.state === 'Expanded')
                    panel.container.style.right = panel.size;
                else
                    panel.container.style.right = `0px`;

                panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                panel.element.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.element.style.width = panel.size;
                panel.stateChanger.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.stateChanger.style.width = "10px";
                panel.stateChanger.style.right = "0px";
            }
        }
    }

    const updateBounds = () => {
        window.RrPage[pageId].bounds = getPageElementBounds(pageId);
        window.RrPage[pageId].panels.forEach(panel => {
            setPanelBounds(panel);
            window.RrPage[pageId].panels.find(p => p.type === 'Right').container.style.zIndex = 15;
            window.RrPage[pageId].panels.find(p => p.type === 'Left').container.style.zIndex = 16;
            window.RrPage[pageId].panels.find(p => p.type === 'Bottom').container.style.zIndex = 17;
            window.RrPage[pageId].panels.find(p => p.type === 'Top').container.style.zIndex = 18;
        });
    };

    window.addEventListener('resize', updateBounds);
    var pageElement = document.getElementById(pageId);
    if (pageElement) {
        new ResizeObserver(updateBounds).observe(pageElement);
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


