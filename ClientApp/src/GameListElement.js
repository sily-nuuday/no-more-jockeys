import React, { Component } from 'react'

class GameListElement extends Component {
	showGame() {
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
            <li>
				<span>{this.props.admin}'s game</span><br />
				<span>Players: {this.props.players}</span><br />
				<span>{this.translateStatus(this.props.status)}</span><br />
				{ notStarted ? <button onClick={this.showGame.bind(this)}>Join</button> : null }
            </li>
        )
    }
	
	async populateGame() {
		const response = await fetch('games/');
		const games = await response.json();
		this.setState((state) => {
            return { games: games };
        });
	}
}

export default GameListElement;
				