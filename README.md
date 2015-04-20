# PowerWallet
This is Bitcoin Wallet for advanced user based on a QBit Ninja Server.

This tool is for advanced users, the ergonomy is inspired on visual studio and does not target average Joe.

Roadmap:
* Transaction Builder interface
* Colored Coin integration (Open Asset)
* Dark wallet integration
* Splitting/Consolidating coins
* Wallet management and tracking (mixing BIP38, BIP32, Stealth Address, normal address, colored or not, multi sig or not)

Power Wallet does not require any synchronization to work, but depends on a QBit Ninja Server.

QBit Ninja is a simple JSON API on the top of NBitcoin.Indexer.
NBitcoin.Indexer is a blockchain indexer that depends on Azure Storage of Microsoft.
The design of NBitcoin.Indexer was thought for scalability and resilience in mind.

For compiling it, git clone, open with visual studio and push F5.
