import React, { Component } from 'react'

class GameListElement extends Component {
    constructor(props) {
        super(props);

        this.joinGame = this.joinGame.bind(this);
	}

	joinGame() {
		this.props.joinHandler(this.props.code);
	}
	
	translateStatus(status) {
		switch (status) {
			case 0:
				return 'Not started yet';
			case 1:
			case 2:
			case 3:
				return 'In progress';
			case 4:
				return 'Completed';
			default:
				return 'Status unknown';
		}
	}
	
	render() {
		const notStarted = this.props.status === 0;
        return (
			<li className="game-list-element">
				<div>
					<div className="left">
						<span>{this.props.admin}'s game</span><br />
						<span>Players: {this.props.players}</span><br />
						<span>{this.translateStatus(this.props.status)}</span><br />
					</div>
					<div className="right">
						<button>Spectate</button><br />
						{ notStarted
							? <button className="game-list-button" onClick={this.joinGame}>Join</button>
							: <button className="game-list-button" disabled="disabled">Join</button> }
					</div>
				</div>
            </li>
        )
    }
}

export default GameListElement;
				