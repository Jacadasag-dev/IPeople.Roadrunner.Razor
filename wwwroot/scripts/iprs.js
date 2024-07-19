﻿window.dropdownInstances = {};
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
    constructor(id, panelElement, dotNetHelper, type, latchingType, container, stateChanger, minLatchingWidth, latching, size, state, sElement1, sElement2, centerElementContainer, sElementContainer1, sElementContainer2, centerElement) {
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
        this.sElement1 = sElement1;
        this.sElement2 = sElement2;
        this.sElementContainer1 = sElementContainer1;
        this.sElementContainer2 = sElementContainer2;
        this.centerElementContainer = centerElementContainer;
        this.centerElement = centerElement;
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
        const centerElementContainer = document.getElementById(`${dto.id}-panel-container-center`);
        const centerElement = document.getElementById(`${dto.id}-panel-center`);
        let sElementContainer1 = document.getElementById(`${dto.id}-Selement-container-1`);
        let sElementContainer2 = document.getElementById(`${dto.id}-Selement-container-2`);
        let sElement1 = document.getElementById(`${dto.id}-Selement-1`);
        let sElement2 = document.getElementById(`${dto.id}-Selement-2`);
        const panel = new RrPanel(dto.id, panelElement, dto.dotNetObjectReference, dto.pType, dto.latchingType, container, stateChanger, dto.minLatchingWidth, dto.latching, dto.size, dto.state, sElement1, sElement2, centerElementContainer, sElementContainer1, sElementContainer2, centerElement);
        panels.push(panel);

        const onMouseDown = (event) => {


            let startY = event.clientY;
            let startX = event.clientX;
            const initialHieght = panel.element.offsetHeight;
            const initialWidth = panel.element.offsetWidth
            const initialStateChangerLeft = panel.stateChanger.offsetLeft;
            const initialStateChangerTop = panel.stateChanger.offsetTop;
            const leftPanelInitialWidth = window.RrPage[pageId].panels.find(p => p.type === 'Left').element.offsetWidth
            let mouseMoved = false;
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
                if (panel.state === 'Collapsed')
                    return;

                mouseMoved = true;

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
                    if (!mouseMoved)
                        toggleUIState(panel);

                    let size = -1;
                    if (panel.type === 'Left' || panel.type === 'Right') {

                        leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                        rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');

                        const leftRect = leftPanel.stateChanger.getBoundingClientRect();
                        const rightRect = rightPanel.stateChanger.getBoundingClientRect();
                        // Check if the left panel intersects with the right panel
                        if (leftPanel.stateChanger.offsetLeft + leftRect.width >= rightRect.left
                            && leftRect.left <= rightRect.left + rightRect.width
                            && leftRect.left + leftRect.width >= rightRect.left)
                        {
                            leftPanel.size = leftPanel.element.style.width;
                            rightPanel.size = rightPanel.element.style.width;
                            addLatching(leftPanel);
                            return;
                        }
                        // Check if the right panel intersects with the left panel
                        else if (rightRect.left <= leftRect.left + leftRect.width
                            && rightRect.left >= leftRect.left
                            && rightRect.left <= leftRect.left + leftRect.width)
                        {
                            leftPanel.size = leftPanel.element.style.width;
                            rightPanel.size = rightPanel.element.style.width;
                            addLatching(rightPanel);
                            return;
                        }

                        size = parseFloat(panel.element.style.width);
                        if (size < 100 || size > window.RrPage[pageId].bounds.width - 20) {
                            toggleUIState(panel);
                            if (panel.type === 'Left') {
                                panel.stateChanger.style.left = panel.size;
                                setTimeout(function () {
                                    panel.element.style.width = panel.size;
                                }, 200);

                            } else {
                                panel.element.style.width = panel.size;
                            }
                        } else {
                            panel.size = `${size}px`;
                        }
                    } else if (panel.type === 'Top' || panel.type === 'Bottom') {
                        size = parseFloat(panel.element.style.height);
                        if (size < 100 || size > window.RrPage[pageId].bounds.height - 20) {
                            toggleUIState(panel);
                            if (panel.type === 'Top') {
                                panel.stateChanger.style.top = panel.size;
                                setTimeout(function () {
                                    panel.element.style.height = panel.size;
                                }, 200);
                            } else {
                                panel.element.style.height = panel.size;
                            }
                        } else {
                            panel.size = `${size}px`;
                        }
                    }
                    panel.enableTransitions();
                }
            };
            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        };

        if (!panel.latching) {
            panel.stateChanger.addEventListener('mousedown', onMouseDown);
        } else {
            panel.sElementContainer1.addEventListener('mousedown', () => removeLatching(panel));
            panel.sElementContainer2.addEventListener('mousedown', () => removeLatching(panel));
            panel.stateChanger.addEventListener('mousedown', onMouseDown);
        }
        panel.element.addEventListener('mousedown', () => focusPanel(panel));
        panel.stateChanger.addEventListener('mousedown', () => focusPanel(panel));
        
    });
    window.RrPage[pageId].panels = panels;

    window.setPanelUIState = function (myPageId, panelId, desiredState) {
        const panel = window.RrPage[myPageId].panels.find(p => p.id === panelId);
        if (panel) {
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
    function addLatching(panel) {
        if (!panel.latching && panel.latchingType === 'Vertical') {
            leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
            rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
            if (leftPanel && rightPanel) {
                leftPanel.latching = true;
                rightPanel.latching = true;
                leftPanel.state = 'Expanded';
                rightPanel.state = 'Expanded';
                setPanelElementTypeClasses(rightPanel);
                setPanelElementTypeClasses(leftPanel);
                setPanelBounds(leftPanel);
                setPanelBounds(rightPanel);
            }
        }
    }
    function removeLatching(panel) {
        if (panel.latching && panel.latchingType === 'Vertical') {
            leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
            rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
            if (leftPanel && rightPanel) {
                leftPanel.latching = false;
                rightPanel.latching = false;
                leftPanel.state = 'Expanded';
                rightPanel.state = 'Expanded';
                setPanelElementTypeClasses(rightPanel);
                setPanelElementTypeClasses(leftPanel);
                setPanelBounds(leftPanel);
                setPanelBounds(rightPanel);
            }
        }
    }
    function toggleUIState(panel, desiredState) {
        const stateToSet = desiredState || (panel.state === 'Collapsed' ? 'Expanded' : 'Collapsed');
        if (panel.latching)
            return;

        if (panel.type === 'Left') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.left = `0px`;
                panel.sElementContainer1.style.display = 'block';
                panel.sElementContainer2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.left = `-${panel.size}`;
                panel.sElementContainer1.style.display = 'none';
                panel.sElementContainer2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Right') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.right = `${panel.size}`;
                panel.sElementContainer1.style.display = 'block';
                panel.sElementContainer2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.right = `0px`;
                panel.sElementContainer1.style.display = 'none';
                panel.sElementContainer2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Top') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.top = `0px`;
                panel.sElementContainer1.style.display = 'block';
                panel.sElementContainer2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.top = `-${panel.size}`;
                panel.sElementContainer1.style.display = 'none';
                panel.sElementContainer2.style.display = 'none';
                panel.makeMinimized();
            }
        } else if (panel.type === 'Bottom') {
            if (stateToSet === 'Expanded') {
                panel.state = 'Expanded';
                panel.container.style.bottom = `${panel.size}`;
                panel.sElementContainer1.style.display = 'block';
                panel.sElementContainer2.style.display = 'block';
                panel.makeExpanded();
            } else if (stateToSet === 'Collapsed') {
                panel.state = 'Collapsed';
                panel.container.style.bottom = `0px`;
                panel.sElementContainer1.style.display = 'none';
                panel.sElementContainer2.style.display = 'none';
                panel.makeMinimized();
            }
        }
        panel.dotNetHelper.invokeMethodAsync('UpdateStateServicePanelState', panel.state).catch(err => console.error(err));
    }
    function setPanelTypeClasses(panel) {
        if (panel) {
            let panelType = panel.type.toLowerCase();
            if (!panel.centerElementContainer.classList.contains(panelType)) panel.centerElementContainer.classList.add(panelType)
            if (!panel.centerElement.classList.contains(panelType)) panel.centerElement.classList.add(panelType)
            if (!panel.sElementContainer1.classList.contains(panelType)) panel.sElementContainer1.classList.add(panelType)
            if (!panel.sElementContainer2.classList.contains(panelType)) panel.sElementContainer2.classList.add(panelType)
            if (!panel.sElement1.classList.contains(panelType)) panel.sElement1.classList.add(panelType)
            if (!panel.sElement2.classList.contains(panelType)) panel.sElement2.classList.add(panelType)
        }
    }
    function setPanelElementTypeClasses(panel) {
        if (panel) {
            if (panel.latching) {
                if (panel.latchingType === 'Vertical') {
                    panel.centerElement.classList.add('dots');
                    panel.centerElementContainer.classList.add('dots');
                    panel.sElement1.classList.add('detacher');
                    panel.sElement2.classList.add('detacher');
                    if (panel.centerElement.classList.contains('arrow')) panel.centerElement.classList.remove('arrow');
                    if (panel.centerElementContainer.classList.contains('arrow')) panel.centerElementContainer.classList.remove('arrow');
                    if (panel.sElement1.classList.contains('dots')) panel.sElement1.classList.remove('dots');
                    if (panel.sElement2.classList.contains('dots')) panel.sElement2.classList.remove('dots');
                }
            }
            else {
                if (panel.latchingType === 'Vertical') {
                    panel.centerElement.classList.add('arrow');
                    panel.centerElementContainer.classList.add('arrow');
                    panel.sElement1.classList.add('dots');
                    panel.sElement2.classList.add('dots');
                    if (panel.centerElementContainer.classList.contains('dots')) panel.centerElementContainer.classList.remove('dots');
                    if (panel.centerElement.classList.contains('dots')) panel.centerElement.classList.remove('dots');
                    if (panel.sElement1.classList.contains('detacher')) panel.sElement1.classList.remove('detacher');
                    if (panel.sElement2.classList.contains('detacher')) panel.sElement2.classList.remove('detacher');
                }
            }
        }
    }
    function setPanelBounds(panel) {
        setPanelTypeClasses(panel);
        if (panel.latching) {
            if (panel.latchingType === 'Vertical') {
                leftPanel = window.RrPage[pageId].panels.find(p => p.type === 'Left');
                rightPanel = window.RrPage[pageId].panels.find(p => p.type === 'Right');
                if (leftPanel && rightPanel) {
                    leftPanel.disableTransitions();
                    rightPanel.disableTransitions();
                    panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                    panel.stateChanger.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.centerElementContainer.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.element.style.height = `${window.RrPage[pageId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.stateChanger.style.width = "10px";
                    leftPanel.container.style.left = "0px";
                    leftPanel.element.style.width = leftPanel.size;
                    leftPanel.stateChanger.style.left = leftPanel.size;
                    rightPanel.container.style.right = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${leftPanel.size})`;
                    rightPanel.element.style.width = `calc(${window.RrPage[pageId].bounds.width - 20}px - ${leftPanel.size})`;
                    rightPanel.stateChanger.style.right = `0px`;
                    leftPanel.container.style.zIndex = 19;
                    rightPanel.container.style.zIndex = 18;
                    setPanelElementTypeClasses(rightPanel);
                    setPanelElementTypeClasses(leftPanel);
                    setPanelTypeClasses(panel);
                }
            }
        } else {
            setPanelElementTypeClasses(panel);
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


