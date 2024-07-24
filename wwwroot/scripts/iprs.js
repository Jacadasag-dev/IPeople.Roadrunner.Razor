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
class ContainingDiv {
    constructor(id, bounds, panels) {
        this.id = id;
        this.bounds = bounds;
        this.panels = panels;
        this.detachable = false;
    }
}
class RrPanel {
    constructor({ id, panelElement, dotNetHelper, type, latchingType, container, stateChanger, minLatchingWidth, latching, size, state, sElement1, sElement2, centerElementContainer, sElementContainer1, sElementContainer2, centerElement }) {
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
        this.state = 'Expanded';
        this.dotNetHelper.invokeMethodAsync('UpdateStateServicePanelState', this.state).catch(err => console.error(err));
    }

    makeMinimized() {
        if (this.stateChanger.classList.contains('expanded')) this.stateChanger.classList.remove('expanded');
        if (this.container.classList.contains('expanded')) this.container.classList.remove('expanded');
        if (this.element.classList.contains('expanded')) this.element.classList.remove('expanded');
        this.stateChanger.classList.add('minimized');
        this.container.classList.add('minimized');
        this.element.classList.add('minimized');
        this.state = 'Collapsed';
        this.dotNetHelper.invokeMethodAsync('UpdateStateServicePanelState', this.state).catch(err => console.error(err));
    }

    setPanelStatechangerElementClasses(containingDivId) {
        if (this) {
            if (this.latching) {
                if (this.latchingType === 'Vertical') {
                    if (!this.container.classList.contains('latching')) this.container.classList.add('latching');
                    if (!this.stateChanger.classList.contains('latching')) this.stateChanger.classList.add('latching');
                    if (!this.stateChanger.classList.contains('vertical')) this.stateChanger.classList.add('vertical');
                    if (!this.centerElement.classList.contains('dots')) this.centerElement.classList.add('dots');
                    if (!this.centerElementContainer.classList.contains('dots')) this.centerElementContainer.classList.add('dots');
                    if (!this.sElement1.classList.contains('detacher')) this.sElement1.classList.add('detacher');
                    if (!this.sElement2.classList.contains('detacher')) this.sElement2.classList.add('detacher');
                    if (this.centerElement.classList.contains('arrow')) this.centerElement.classList.remove('arrow');
                    if (this.centerElementContainer.classList.contains('arrow')) this.centerElementContainer.classList.remove('arrow');
                    if (this.sElement1.classList.contains('dots')) this.sElement1.classList.remove('dots');
                    if (this.sElement2.classList.contains('dots')) this.sElement2.classList.remove('dots');
                    if (window.ContainingDivs[containingDivId].detachable) {
                        this.sElement1.style.display = "block";
                        this.sElement2.style.display = "block";
                    } else {
                        this.sElement1.style.display = "none";
                        this.sElement2.style.display = "none";
                    }
                }
            }
            else {
                if (this.container.classList.contains('latching')) this.container.classList.remove('latching');
                if (this.stateChanger.classList.contains('latching')) this.stateChanger.classList.remove('latching');
                if (this.stateChanger.classList.contains('vertical')) this.stateChanger.classList.remove('vertical');
                if (!this.centerElement.classList.contains('arrow')) this.centerElement.classList.add('arrow');
                if (!this.centerElementContainer.classList.contains('arrow')) this.centerElementContainer.classList.add('arrow');
                if (!this.sElement1.classList.contains('dots')) this.sElement1.classList.add('dots');
                if (!this.sElement2.classList.contains('dots')) this.sElement2.classList.add('dots');
                if (this.centerElementContainer.classList.contains('dots')) this.centerElementContainer.classList.remove('dots');
                if (this.centerElement.classList.contains('dots')) this.centerElement.classList.remove('dots');
                if (this.sElement1.classList.contains('detacher')) this.sElement1.classList.remove('detacher');
                if (this.sElement2.classList.contains('detacher')) this.sElement2.classList.remove('detacher');
            }
        }
    }

    setPanelTypeClasses() {
        if (this) {
            let panelType = this.type.toLowerCase();
            if (!this.centerElementContainer.classList.contains(panelType)) this.centerElementContainer.classList.add(panelType)
            if (!this.centerElement.classList.contains(panelType)) this.centerElement.classList.add(panelType)
            if (!this.sElementContainer1.classList.contains(panelType)) this.sElementContainer1.classList.add(panelType)
            if (!this.sElementContainer2.classList.contains(panelType)) this.sElementContainer2.classList.add(panelType)
            if (!this.sElement1.classList.contains(panelType)) this.sElement1.classList.add(panelType)
            if (!this.sElement2.classList.contains(panelType)) this.sElement2.classList.add(panelType)
        }
    }
}

window.ContainingDivs = {};

window.getPanelContainingDivId = function (panelId) {
    const container = document.getElementById(`${panelId}-panel-container`);
    if (!container) {
        throw new Error(`Container with id ${panelId}-panel-container not found.`);
    }

    const panelContainingDiv = container.parentElement;
    if (!panelContainingDiv) {
        throw new Error(`Parent element of container with id ${panelId}-panel-container not found.\nThe containing div of a RrPanel component bust have an id.`);
    }

    const panelContainingDivId = panelContainingDiv.id;
    if (!panelContainingDivId) {
        throw new Error(`Parent element of container with id ${panelId}-panel-container does not have an id.\nThe containing div of a RrPanel component bust have an id.`);
    }
    return panelContainingDivId;
};

window.setContainingDivDetachable = function (containingDivId, detachable) {
    let containingDiv = window.ContainingDivs[containingDivId];
    if (containingDiv) {
        containingDiv.detachable = detachable;
    }
};

window.registerContainingDivAndPanels = function (panelDtos, containingDivId = '') {
    if (!panelDtos || panelDtos.length === 0) {
        throw new Error(`Panels could not be registered because no panel objects were passed to registerContainingDivAndPanels.`);
    }
    if (!containingDivId) {
        const firstDto = panelDtos[0];
        containingDivId = window.getPanelContainingDivId(firstDto.id);
    }

    // Register ContainingDiv
    if (!window.ContainingDivs[containingDivId]) {
        window.ContainingDivs[containingDivId] = new ContainingDiv(containingDivId, null, []);
        window.ContainingDivs[containingDivId].bounds = getContainingDivElementBounds(containingDivId);
        if (!window.ContainingDivs[containingDivId].bounds) {
            let containingDiv = document.getElementById(containingDivId);
            if (!containingDiv) {
                throw new Error(`ContainingDiv with id: "${containingDivId}" not found.`);
            } else {
                throw new Error(`ContainingDiv with id: "${containingDivId}" does not have bounds.`);
            }
        }
    }
    let justlatched = false;
    // Register Panels
    var panels = [];
    panelDtos.forEach(function (dto) {
        // throw an error if any of the elements are not found
        const panelElement = document.getElementById(`${dto.id}-panel`);
        const container = document.getElementById(`${dto.id}-panel-container`);
        const stateChanger = document.getElementById(`${dto.id}-panel-statechanger`);
        const centerElementContainer = document.getElementById(`${dto.id}-panel-container-center`);
        const centerElement = document.getElementById(`${dto.id}-panel-center`);
        const sElementContainer1 = document.getElementById(`${dto.id}-Selement-container-1`);
        const sElementContainer2 = document.getElementById(`${dto.id}-Selement-container-2`);
        const sElement1 = document.getElementById(`${dto.id}-Selement-1`);
        const sElement2 = document.getElementById(`${dto.id}-Selement-2`);
        if (!panelElement) throw new Error(`Element with id ${dto.id}-panel not found.`);
        if (!container) throw new Error(`Element with id ${dto.id}-panel-container not found.`);
        if (!stateChanger) throw new Error(`Element with id ${dto.id}-panel-statechanger not found.`);
        if (!centerElementContainer) throw new Error(`Element with id ${dto.id}-panel-container-center not found.`);
        if (!centerElement) throw new Error(`Element with id ${dto.id}-panel-center not found.`);
        if (!sElementContainer1) throw new Error(`Element with id ${dto.id}-Selement-container-1 not found.`);
        if (!sElementContainer2) throw new Error(`Element with id ${dto.id}-Selement-container-2 not found.`);
        if (!sElement1) throw new Error(`Element with id ${dto.id}-Selement-1 not found.`);
        if (!sElement2) throw new Error(`Element with id ${dto.id}-Selement-2 not found.`);

        const panel = new RrPanel({
            id: dto.id,
            panelElement: panelElement,
            dotNetHelper: dto.dotNetObjectReference,
            type: dto.pType,
            latchingType: dto.latchingType,
            container: container,
            stateChanger: stateChanger,
            minLatchingWidth: dto.minLatchingWidth,
            latching: dto.latching,
            size: dto.size,
            state: dto.state,
            sElement1: sElement1,
            sElement2: sElement2,
            centerElementContainer: centerElementContainer,
            sElementContainer1: sElementContainer1,
            sElementContainer2: sElementContainer2,
            centerElement: centerElement
        });
        panels.push(panel);

        const onMouseDown = (event) => {
            let startY = event.clientY;
            let startX = event.clientX;
            const initialHieght = panel.element.offsetHeight;
            const initialWidth = panel.element.offsetWidth
            const initialStateChangerLeft = panel.stateChanger.offsetLeft;
            const initialStateChangerTop = panel.stateChanger.offsetTop;
            const myLeftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
            const leftPanelInitialWidth = myLeftPanel?.element.offsetWidth;
            let mouseMoved = false;
            panel.disableTransitions();

            const onMouseMove = (event) => {
                if (panel.state === 'Collapsed')
                    return;

                mouseMoved = true;
                const diffX = event.clientX - startX;
                const diffY = event.clientY - startY;
                if (panel.latching) {
                    if (panel.latchingType === 'Vertical') {
                        let leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
                        let rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
                        if (leftPanel && rightPanel) {
                            // Calculate the new width while respecting the minimum latching width
                            let newLeftPanelWidth = leftPanelInitialWidth + diffX;
                            let newRightPanelWidth = window.ContainingDivs[containingDivId].bounds.width - 20 - newLeftPanelWidth;

                            if (newLeftPanelWidth < leftPanel.minLatchingWidth) {
                                newLeftPanelWidth = leftPanel.minLatchingWidth;
                                newRightPanelWidth = window.ContainingDivs[containingDivId].bounds.width - 20 - newLeftPanelWidth;
                            }

                            if (newRightPanelWidth < rightPanel.minLatchingWidth) {
                                newRightPanelWidth = rightPanel.minLatchingWidth;
                                newLeftPanelWidth = window.ContainingDivs[containingDivId].bounds.width - 20 - newRightPanelWidth;
                            }

                            // Apply the new widths
                            leftPanel.element.style.width = `${newLeftPanelWidth}px`;
                            leftPanel.stateChanger.style.left = `${newLeftPanelWidth}px`;
                            rightPanel.container.style.right = `calc(${window.ContainingDivs[containingDivId].bounds.width - 20}px - ${newLeftPanelWidth}px)`;
                            rightPanel.element.style.width = `calc(${window.ContainingDivs[containingDivId].bounds.width - 20}px - ${newLeftPanelWidth}px)`;
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
                panel.enableTransitions();
                if (panel.latching) {
                    if (panel.latchingType === 'Vertical') {
                        let leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
                        let rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
                        if (leftPanel && rightPanel) {
                            leftPanel.size = `${leftPanel.element.offsetWidth}px`;
                            rightPanel.size = `${rightPanel.element.offsetWidth}px`;
                        }
                    }
                } else {
                    if (!mouseMoved) {
                        if (!justlatched) {
                            toggleUIState(panel);
                        } else {
                            if (panel.latchingType === 'Vertical' && (panel.type === 'Top' || panel.type === 'Bottom')) {
                                toggleUIState(panel);
                            }
                        }
                    }
                    let size = -1;
                    if (panel.type === 'Left' || panel.type === 'Right') {
                        if (!justlatched && panel.latchingType === 'Vertical') {
                            let leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
                            let rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
                            const leftRect = leftPanel.stateChanger.getBoundingClientRect();
                            const rightRect = rightPanel.stateChanger.getBoundingClientRect();
                            // Check if the left panel intersects with the right panel
                            if (leftPanel.stateChanger.offsetLeft + leftRect.width >= rightRect.left
                                && leftRect.left <= rightRect.left + rightRect.width
                                && leftRect.left + leftRect.width >= rightRect.left)
                            {
                                if (leftPanel.state === 'Expanded' && rightPanel.state === 'Expanded') {
                                    leftPanel.size = `${leftPanel.element.offsetWidth}px`;
                                    rightPanel.size = `${rightPanel.element.offsetWidth}px`;
                                    setLatching(leftPanel);
                                    return;
                                }
                            }
                            // Check if the right panel intersects with the left panel
                            else if (rightRect.left <= leftRect.left + leftRect.width
                                && rightRect.left >= leftRect.left
                                && rightRect.left <= leftRect.left + leftRect.width)
                            {
                                if (leftPanel.state === 'Expanded' && rightPanel.state === 'Expanded') {
                                    leftPanel.size = `${leftPanel.element.offsetWidth}px`;
                                    rightPanel.size = `${rightPanel.element.offsetWidth}px`;
                                    setLatching(leftPanel);
                                    return;
                                }
                            }
                        }
                        size = parseFloat(panel.element.style.width);
                        if (size < 50 || size > window.ContainingDivs[containingDivId].bounds.width - 20) {
                            if (panel.type === 'Left') {
                                panel.container.style.transition = 'none';
                                panel.stateChanger.style.left = panel.size;
                                panel.element.style.width = panel.size;
                            } else {
                                panel.element.style.width = panel.size;
                            }
                            toggleUIState(panel);
                            
                        } else {
                            panel.size = `${size}px`;
                        }
                    } else if (panel.type === 'Top' || panel.type === 'Bottom') {
                        size = parseFloat(panel.element.style.height);
                        if (size < 50 || size > window.ContainingDivs[containingDivId].bounds.height - 20) {
                            if (panel.type === 'Top') {
                                panel.container.style.transition = 'none';
                                panel.stateChanger.style.top = panel.size;
                                panel.element.style.height = panel.size;
                            } else {
                                panel.element.style.height = panel.size;
                            }
                            toggleUIState(panel);
                        } else {
                            panel.size = `${size}px`;
                        }
                    }
                    panel.enableTransitions();
                    justlatched = false;
                    setTimeout(function () {
                        panel.container.style.transition = '';
                    }, 1);
                }
            };
            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        };

        if (panel.latching) {
            panel.sElementContainer1.addEventListener('mousedown', () => setNotLatching(panel));
            panel.sElementContainer2.addEventListener('mousedown', () => setNotLatching(panel));
        }
        panel.stateChanger.addEventListener('mousedown', (event) => { onMouseDown(event); });
        panel.element.addEventListener('mousedown', () => setFocusPanelOrder(panel));
        
    });

    window.ContainingDivs[containingDivId].panels = panels;
    window.setPanelUIState = function (myContainingDivId, panelId, desiredState) {
        const panel = window.ContainingDivs[myContainingDivId].panels.find(p => p.id === panelId);
        if (panel) {
            toggleUIState(panel, desiredState);
        }
    };
    function toggleUIState(panel, desiredState) {
        if (panel.latching)
            return;

        if (desiredState) {
            if (desiredState === 'Expanded') {
                panel.makeExpanded();
                
            } else {
                panel.makeMinimized();
            }
        } else {
            if (panel.state === 'Expanded') {
                panel.makeMinimized();
            }
            else {
                panel.makeExpanded();
            }
        }
        setFocusPanelOrder(panel);
    }
    function getContainingDivElementBounds(elementId) {
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
        var bottomPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Bottom');
        var topPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Top');
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Expanded') 
            offset = 0;

        if (bottomPanel && bottomPanel.state === 'Collapsed' && topPanel && topPanel.state === 'Collapsed') 
            offset = -20;
        
        if (bottomPanel && bottomPanel.state === 'Collapsed' && topPanel && topPanel.state === 'Expanded') 
            offset = -10;
        
        if (bottomPanel && bottomPanel.state === 'Expanded' && topPanel && topPanel.state === 'Collapsed') 
            offset = -10;

        // adjust for possible border widths
        const computedStyle = window.getComputedStyle(document.getElementById(window.ContainingDivs[containingDivId].id));
        const containingDivBorderBottomWidth = computedStyle.borderBottomWidth;
        const containingDivBorderTopWidth = computedStyle.borderTopWidth;
        if (containingDivBorderBottomWidth) {
            offset -= parseFloat(containingDivBorderBottomWidth);
        }
        if (containingDivBorderTopWidth) {
            offset -= parseFloat(containingDivBorderTopWidth);
        }

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
        var bottomPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Bottom');
        var topPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Top');
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
    function setFocusPanelOrder(panel) {
        const panels = window.ContainingDivs[containingDivId].panels;
        if (!panel.latching) {
            panels.forEach(p => {
                p.isFocused = (p.id === panel.id);
                if (p.state === 'Collapsed') {
                    p.container.style.zIndex = 20;
                }
            });
            panels.sort((a, b) => {
                if (a.isFocused && !b.isFocused) return -1;
                if (!a.isFocused && b.isFocused) return 1;
                return 0;
            });
            panels.forEach((p, i) => {
                if (p.state === 'Expanded') {
                    p.container.style.zIndex = `${20 + (panels.length - i)}`;
                }
                setPanelBounds(p);
            });
        }
    };
    function setLatching(panel) {
        if (!panel.latching && panel.latchingType === 'Vertical') {
            leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
            rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
            if (leftPanel && rightPanel) {
                leftPanel.latching = true;
                rightPanel.latching = true;
                setPanelBounds(leftPanel);
                setPanelBounds(rightPanel);
                justlatched = true;
            }
        }
    }
    function setNotLatching(panel) {
        if (window.ContainingDivs[containingDivId].detachable && panel.latching && panel.latchingType === 'Vertical') {
            leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
            rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
            if (leftPanel && rightPanel) {
                leftPanel.latching = false;
                rightPanel.latching = false;
                leftPanel.enableTransitions();
                rightPanel.enableTransitions();
                leftPanel.makeExpanded();
                rightPanel.makeExpanded();
                setPanelBounds(leftPanel);
                setPanelBounds(rightPanel);
            }
        }
    }
    function setPanelBounds(panel) {
        panel.setPanelTypeClasses();
        if (panel.latching) {
            if (panel.latchingType === 'Vertical') {
                leftPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Left');
                rightPanel = window.ContainingDivs[containingDivId].panels.find(p => p.type === 'Right');
                if (leftPanel && rightPanel) {
                    leftPanel.makeExpanded();
                    rightPanel.makeExpanded();
                    panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                    panel.stateChanger.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.element.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                    panel.stateChanger.style.width = "10px";
                    leftPanel.container.style.left = "0px";
                    leftPanel.element.style.width = leftPanel.size;
                    leftPanel.stateChanger.style.left = leftPanel.size;
                    rightPanel.container.style.right = `calc(${window.ContainingDivs[containingDivId].bounds.width - 20}px - ${leftPanel.size})`;
                    rightPanel.element.style.width = `calc(${window.ContainingDivs[containingDivId].bounds.width - 20}px - ${leftPanel.size})`;
                    rightPanel.stateChanger.style.right = `0px`;
                    leftPanel.container.style.zIndex = 19;
                    rightPanel.container.style.zIndex = 18;
                    leftPanel.size = `${leftPanel.element.offsetWidth}px`;
                    rightPanel.size = `${rightPanel.element.offsetWidth}px`;
                    rightPanel.setPanelStatechangerElementClasses(containingDivId);
                    leftPanel.setPanelStatechangerElementClasses(containingDivId);
                }
            }
        } else {
            panel.setPanelStatechangerElementClasses(containingDivId);
            if (panel.type === 'Top') {
                if (panel.state === 'Expanded') {
                    panel.container.style.top = `${getHorizontalPanelLeftOffsets(panel)}px`;
                } else {
                    panel.container.style.top = `-${panel.size}`;
                }
                panel.container.style.left = `0px`;
                panel.element.style.width = `${window.ContainingDivs[containingDivId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.element.style.height = panel.size;
                panel.stateChanger.style.width = `${window.ContainingDivs[containingDivId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.stateChanger.style.height = "10px";
                panel.stateChanger.style.top = panel.size;
            } else if (panel.type === 'Bottom') {
                if (panel.state === 'Expanded') {
                    panel.container.style.bottom = panel.size;
                } else {
                    panel.container.style.bottom = `0px`;
                }
                panel.container.style.left = `${getHorizontalPanelLeftOffsets(panel)}px`;
                panel.element.style.width = `${window.ContainingDivs[containingDivId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.element.style.height = panel.size;
                panel.stateChanger.style.width = `${window.ContainingDivs[containingDivId].bounds.width + getHorizontalPanelWidthOffsets(panel)}px`;
                panel.stateChanger.style.height = "10px";
                panel.stateChanger.style.bottom = "0px";
            } else if (panel.type === 'Left') {
                if (panel.state === 'Expanded') {
                    panel.container.style.left = "0px";
                } else {
                    panel.container.style.left = `-${panel.size}`;
                }
                panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                panel.element.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.element.style.width = panel.size;
                panel.stateChanger.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.stateChanger.style.width = "10px";
                panel.stateChanger.style.left = panel.size;
            } else if (panel.type === 'Right') {
                if (panel.state === 'Expanded') {
                    panel.container.style.right = panel.size;
                } else {
                    panel.container.style.right = `0px`;
                }
                panel.container.style.top = `${0 + getVerticalPanelTopOffsets(panel)}px`;
                panel.element.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.element.style.width = panel.size;
                panel.stateChanger.style.height = `${window.ContainingDivs[containingDivId].bounds.height + getVerticalPanelHeightOffsets(panel)}px`;
                panel.stateChanger.style.width = "10px";
                panel.stateChanger.style.right = "0px";
            }
        }
    }

    const updateBounds = () => {
        window.ContainingDivs[containingDivId].bounds = getContainingDivElementBounds(containingDivId);
        window.ContainingDivs[containingDivId].panels.forEach(panel => {
            if (panel.state !== 'Expanded' && panel.state !== 'Collapsed')
                panel.makeMinimized();

            setPanelBounds(panel);
            if (panel.latching)
                justlatched = true;
        });
    };

    window.addEventListener('resize', updateBounds);
    var pageElement = document.getElementById(containingDivId);
    if (pageElement) {
        new ResizeObserver(updateBounds).observe(pageElement);
    }
};



