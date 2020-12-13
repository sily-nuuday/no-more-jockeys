import React, { Component } from 'react'

class PlayerListElement extends Component {
    render() {
        var element;
        if (this.props.status === 0 || this.props.status === 4) {
            element = <li className="game-list-element">{this.props.player.name}</li>;
        } else if (this.props.player.challengesRemaining === 0) {
            element = <li className="game-list-element player-dead">{this.props.player.name}</li>;
        } else {
            element = <li className="game-list-element">{this.props.player.name} ({this.props.player.challengesRemaining})</li>;
        }
        return element;
    }
}

export default PlayerListElement;