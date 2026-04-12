(function initializeInitiativeBridge(globalScope) {
  var bridgeEventName = 'initiative:talespire-chat-message';

  function dispatchBridgeEvent(eventData) {
    if (!globalScope || typeof globalScope.dispatchEvent !== 'function') {
      return;
    }

    globalScope.dispatchEvent(new CustomEvent(bridgeEventName, {
      detail: eventData,
    }));
  }

  globalScope.handleInitiativeChatMessage = function handleInitiativeChatMessage(eventData, payload) {
    if (typeof payload !== 'undefined') {
      dispatchBridgeEvent({
        kind: eventData,
        payload: payload,
      });
      return;
    }

    dispatchBridgeEvent(eventData);
  };
})(window);