import React, { Component } from 'react'
import PlayerListElement from './PlayerListElement.js'

class PlayerList extends Component {
    render() {
        return (
            <ul className="game-list">
                {this.props.game.players.map((player, index) => {
                    return <PlayerListElement status={this.props.game.status} player={player} key={index} />;
                })}
            </ul>
        );
    }
}

export default PlayerList;