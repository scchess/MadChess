// +------------------------------------------------------------------------------+
// |                                                                              |
// |     MadChess is developed by Erik Madsen.  Copyright 2020.                   |
// |     MadChess is free software.  It is distributed under the GNU General      |
// |     Public License Version 3 (GPLv3).  See LICENSE file for details.         |
// |     See https://www.madchess.net/ for user and developer guides.             |
// |                                                                              |
// +------------------------------------------------------------------------------+


using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace ErikTheCoder.MadChess.Engine
{
    public static class Move
    {
        public static readonly ulong Null;
        private static readonly int _bestShift;
        private static readonly ulong _bestMask;
        private static readonly ulong _bestUnmask;
        private static readonly int _captureVictimShift;
        private static readonly ulong _captureVictimMask;
        private static readonly ulong _captureVictimUnmask;
        private static readonly int _captureAttackerShift;
        private static readonly ulong _captureAttackerMask;
        private static readonly ulong _captureAttackerUnmask;
        private static readonly int _promotedPieceShift;
        private static readonly ulong _promotedPieceMask;
        private static readonly ulong _promotedPieceUnmask;
        private static readonly int _killerShift;
        private static readonly ulong _killerMask;
        private static readonly ulong _killerUnmask;
        private static readonly int _historyShift;
        private static readonly ulong _historyMask;
        private static readonly ulong _historyUnmask;
        private static readonly int _playedShift;
        private static readonly ulong _playedMask;
        private static readonly ulong _playedUnmask;
        private static readonly int _castlingShift;
        private static readonly ulong _castlingMask;
        private static readonly ulong _castlingUnmask;
        private static readonly int _kingMoveShift;
        private static readonly ulong _kingMoveMask;
        private static readonly ulong _kingMoveUnmask;
        private static readonly int _enPassantShift;
        private static readonly ulong _enPassantMask;
        private static readonly ulong _enPassantUnmask;
        private static readonly int _pawnMoveShift;
        private static readonly ulong _pawnMoveMask;
        private static readonly ulong _pawnMoveUnmask;
        private static readonly int _checkShift;
        private static readonly ulong _checkMask;
        private static readonly ulong _checkUnmask;
        private static readonly int _doublePawnMoveShift;
        private static readonly ulong _doublePawnMoveMask;
        private static readonly ulong _doublePawnMoveUnmask;
        private static readonly int _quietShift;
        private static readonly ulong _quietMask;
        private static readonly ulong _quietUnmask;
        private static readonly int _fromShift;
        private static readonly ulong _fromMask;
        private static readonly ulong _fromUnmask;
        private static readonly ulong _toMask;
        private static readonly ulong _toUnmask;


        // Move Bits
        // Higher priority moves have higher ulong value.

        // 6 6 6 6 5 5 5 5 5 5 5 5 5 5 4 4 4 4 4 4 4 4 4 4 3 3 3 3 3 3 3 3 3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0
        // 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
        // B|CapV   |CapA   |Promo  |Kil|History                                              |!|O|K|E|2|P|C|Q|From         |To           
        //                               1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1

        // B =     Best Move
        // CapV =  Capture Victim
        // CapA =  Capture Attacker (inverted)
        // Promo = Promoted Piece
        // Kil =   Killer Move
        // ! =     Played
        // O =     Castling
        // K =     King Move
        // E =     En Passant Capture
        // 2 =     Double Pawn Move
        // P =     Pawn Move
        // C =     Check
        // Q =     Quiet (not capture, pawn promotion, castling, or check)
        // From =  From (one extra bit for illegal square)
        // To =    To (one extra bit for illegal square)


        static Move()
        {
            // Create bit masks and shifts.
            _bestShift = 63;
            _bestMask = Bitwise.CreateULongMask(63);
            _bestUnmask = Bitwise.CreateULongUnmask(63);
            _captureVictimShift = 59;
            _captureVictimMask = Bitwise.CreateULongMask(59, 62);
            _captureVictimUnmask = Bitwise.CreateULongUnmask(59, 62);
            _captureAttackerShift = 55;
            _captureAttackerMask = Bitwise.CreateULongMask(55, 58);
            _captureAttackerUnmask = Bitwise.CreateULongUnmask(55, 58);
            _promotedPieceShift = 51;
            _promotedPieceMask = Bitwise.CreateULongMask(51, 54);
            _promotedPieceUnmask = Bitwise.CreateULongUnmask(51, 54);
            _killerShift = 49;
            _killerMask = Bitwise.CreateULongMask(49, 50);
            _killerUnmask = Bitwise.CreateULongUnmask(49, 50);
            _historyShift = 22;
            _historyMask = Bitwise.CreateULongMask(22, 48);
            _historyUnmask = Bitwise.CreateULongUnmask(22, 48);
            _playedShift = 21;
            _playedMask = Bitwise.CreateULongMask(21);
            _playedUnmask = Bitwise.CreateULongUnmask(21);
            _castlingShift = 20;
            _castlingMask = Bitwise.CreateULongMask(20);
            _castlingUnmask = Bitwise.CreateULongUnmask(20);
            _kingMoveShift = 19;
            _kingMoveMask = Bitwise.CreateULongMask(19);
            _kingMoveUnmask = Bitwise.CreateULongUnmask(19);
            _enPassantShift = 18;
            _enPassantMask = Bitwise.CreateULongMask(18);
            _enPassantUnmask = Bitwise.CreateULongUnmask(18);
            _doublePawnMoveShift = 17;
            _doublePawnMoveMask = Bitwise.CreateULongMask(17);
            _doublePawnMoveUnmask = Bitwise.CreateULongUnmask(17);
            _pawnMoveShift = 16;
            _pawnMoveMask = Bitwise.CreateULongMask(16);
            _pawnMoveUnmask = Bitwise.CreateULongUnmask(16);
            _checkShift = 15;
            _checkMask = Bitwise.CreateULongMask(15);
            _checkUnmask = Bitwise.CreateULongUnmask(15);
            _quietShift = 14;
            _quietMask = Bitwise.CreateULongMask(14);
            _quietUnmask = Bitwise.CreateULongUnmask(14);
            _fromShift = 7;
            _fromMask = Bitwise.CreateULongMask(7, 13);
            _fromUnmask = Bitwise.CreateULongUnmask(7, 13);
            _toMask = Bitwise.CreateULongMask(0, 6);
            _toUnmask = Bitwise.CreateULongUnmask(0, 6);
            // Set null move.
            Null = 0;
            SetIsBest(ref Null, false);
            SetCaptureVictim(ref Null, Piece.None);
            SetCaptureAttacker(ref Null, Piece.None);
            SetPromotedPiece(ref Null, Piece.None);
            SetKiller(ref Null, 0);
            SetHistory(ref Null, 0);
            SetPlayed(ref Null, false);
            SetIsCastling(ref Null, false);
            SetIsKingMove(ref Null, false);
            SetIsEnPassantCapture(ref Null, false);
            SetIsDoublePawnMove(ref Null, false);
            SetIsPawnMove(ref Null, false);
            SetIsCheck(ref Null, false);
            SetIsQuiet(ref Null, false);
            SetFrom(ref Null, Square.Illegal);
            SetTo(ref Null, Square.Illegal);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBest(ulong Move) => (Move & _bestMask) >> _bestShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsBest(ref ulong Move, bool IsBest)
        {
            var isBest = IsBest ? 1ul : 0;
            // Clear
            Move &= _bestUnmask;
            // Set
            Move |= (isBest << _bestShift) & _bestMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsBest(Move) == IsBest);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CaptureVictim(ulong Move) => (int)((Move & _captureVictimMask) >> _captureVictimShift);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCaptureVictim(ref ulong Move, int CaptureVictim)
        {
            // Clear
            Move &= _captureVictimUnmask;
            // Set
            Move |= ((ulong)CaptureVictim << _captureVictimShift) & _captureVictimMask;
            // Validate move.
            Debug.Assert(Engine.Move.CaptureVictim(Move) == CaptureVictim);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CaptureAttacker(ulong Move)
        {
            // Value is inverted.
            var storedPiece = (int)((Move & _captureAttackerMask) >> _captureAttackerShift);
            return 12 - storedPiece;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCaptureAttacker(ref ulong Move, int CaptureAttacker)
        {
            // Invert piece value so P x Q captures are given a higher priority than Q x Q.
            var storedPiece = (ulong)(12 - CaptureAttacker);
            // Clear
            Move &= _captureAttackerUnmask;
            // Set
            Move |= (storedPiece << _captureAttackerShift) & _captureAttackerMask;
            // Validate move.
            Debug.Assert(Engine.Move.CaptureAttacker(Move) == CaptureAttacker);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PromotedPiece(ulong Move) => (int)((Move & _promotedPieceMask) >> _promotedPieceShift);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPromotedPiece(ref ulong Move, int PromotedPiece)
        {
            // Clear
            Move &= _promotedPieceUnmask;
            // Set
            Move |= ((ulong)PromotedPiece << _promotedPieceShift) & _promotedPieceMask;
            // Validate move.
            Debug.Assert(Engine.Move.PromotedPiece(Move) == PromotedPiece);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Killer(ulong Move) => (int)((Move & _killerMask) >> _killerShift);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetKiller(ref ulong Move, int Killer)
        {
            // Clear
            Move &= _killerUnmask;
            // Set
            Move |= ((ulong)Killer << _killerShift) & _killerMask;
            // Validate move.
            Debug.Assert(Engine.Move.Killer(Move) == Killer);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int History(ulong Move) => (int)((Move & _historyMask) >> _historyShift) - MoveHistory.MaxValue;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetHistory(ref ulong Move, int History)
        {
            // Ensure history is >= 0 before shifting into ulong.
            var history = History + MoveHistory.MaxValue;
            // Clear
            Move &= _historyUnmask;
            // Set
            Move |= ((ulong)history << _historyShift) & _historyMask;
            // Validate move.
            Debug.Assert(Engine.Move.History(Move) == History);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Played(ulong Move) => (Move & _playedMask) >> _playedShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPlayed(ref ulong Move, bool Played)
        {
            var played = Played ? 1ul : 0;
            // Clear
            Move &= _playedUnmask;
            // Set
            Move |= (played << _playedShift) & _playedMask;
            // Validate move.
            Debug.Assert(Engine.Move.Played(Move) == Played);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCastling(ulong Move) => (Move & _castlingMask) >> _castlingShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsCastling(ref ulong Move, bool IsCastling)
        {
            var isCastling = IsCastling ? 1ul : 0;
            // Clear
            Move &= _castlingUnmask;
            // Set
            Move |= (isCastling << _castlingShift) & _castlingMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsCastling(Move) == IsCastling);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKingMove(ulong Move) => (Move & _kingMoveMask) >> _kingMoveShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsKingMove(ref ulong Move, bool IsKingMove)
        {
            var isKingMove = IsKingMove ? 1ul : 0;
            // Clear
            Move &= _kingMoveUnmask;
            // Set
            Move |= (isKingMove << _kingMoveShift) & _kingMoveMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsKingMove(Move) == IsKingMove);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnPassantCapture(ulong Move) => (Move & _enPassantMask) >> _enPassantShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsEnPassantCapture(ref ulong Move, bool IsEnPassantCapture)
        {
            var isEnPassantCapture = IsEnPassantCapture ? 1ul : 0;
            // Clear
            Move &= _enPassantUnmask;
            // Set
            Move |= (isEnPassantCapture << _enPassantShift) & _enPassantMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsEnPassantCapture(Move) == IsEnPassantCapture);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPawnMove(ulong Move) => (Move & _pawnMoveMask) >> _pawnMoveShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsPawnMove(ref ulong Move, bool IsPawnMove)
        {
            var isPawnMove = IsPawnMove ? 1ul : 0;
            // Clear
            Move &= _pawnMoveUnmask;
            // Set
            Move |= (isPawnMove << _pawnMoveShift) & _pawnMoveMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsPawnMove(Move) == IsPawnMove);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCheck(ulong Move) => (Move & _checkMask) >> _checkShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsCheck(ref ulong Move, bool IsCheck)
        {
            var isCheck = IsCheck ? 1ul : 0;
            // Clear
            Move &= _checkUnmask;
            // Set
            Move |= (isCheck << _checkShift) & _checkMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsCheck(Move) == IsCheck);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDoublePawnMove(ulong Move) => (Move & _doublePawnMoveMask) >> _doublePawnMoveShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsDoublePawnMove(ref ulong Move, bool IsDoublePawnMove)
        {
            var isDoublePawnMove = IsDoublePawnMove ? 1ul : 0;
            // Clear
            Move &= _doublePawnMoveUnmask;
            // Set
            Move |= (isDoublePawnMove << _doublePawnMoveShift) & _doublePawnMoveMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsDoublePawnMove(Move) == IsDoublePawnMove);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuiet(ulong Move) => (Move & _quietMask) >> _quietShift > 0;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIsQuiet(ref ulong Move, bool IsQuiet)
        {
            var isQuiet = IsQuiet ? 1ul : 0;
            // Clear
            Move &= _quietUnmask;
            // Set
            Move |= (isQuiet << _quietShift) & _quietMask;
            // Validate move.
            Debug.Assert(Engine.Move.IsQuiet(Move) == IsQuiet);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int From(ulong Move) => (int)((Move & _fromMask) >> _fromShift);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFrom(ref ulong Move, int From)
        {
            // Clear
            Move &= _fromUnmask;
            // Set
            Move |= ((ulong)From << _fromShift) & _fromMask;
            // Validate move.
            Debug.Assert(Engine.Move.From(Move) == From);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int To(ulong Move) => (int)(Move & _toMask);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTo(ref ulong Move, int To)
        {
            // Clear
            Move &= _toUnmask;
            // Set
            Move |= (ulong)To & _toMask;
            // Validate move.
            Debug.Assert(Engine.Move.To(Move) == To);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ulong Move1, ulong Move2) => (From(Move1) == From(Move2)) && (To(Move1) == To(Move2)) && (PromotedPiece(Move1) == PromotedPiece(Move2));


        public static ulong ParseLongAlgebraic(string LongAlgebraic, bool WhiteMove)
        {
            var fromSquare = Board.GetSquare(LongAlgebraic.Substring(0, 2));
            var toSquare = Board.GetSquare(LongAlgebraic.Substring(2, 2));
            // Set case of promoted piece character based on side to move.
            var promotedPiece = LongAlgebraic.Length == 5
                ? Piece.ParseChar(WhiteMove ? char.ToUpper(LongAlgebraic[4]) : char.ToLower(LongAlgebraic[4]))
                : Piece.None;
            var move = Null;
            SetFrom(ref move, fromSquare);
            SetTo(ref move, toSquare);
            SetPromotedPiece(ref move, promotedPiece);
            return move;
        }


        public static ulong ParseStandardAlgebraic(Board Board, string StandardAlgebraic)
        {
            var move = Null;
            // Remove check and checkmate symbols.
            var standardAlgebraicNoCheck = StandardAlgebraic.TrimEnd("+#".ToCharArray());
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (standardAlgebraicNoCheck)
            {
                case "O-O-O":
                case "0-0-0":
                    if (Board.CurrentPosition.WhiteMove)
                    {
                        // White Castle Queenside
                        SetFrom(ref move, Square.e1);
                        SetTo(ref move, Square.c1);
                        if (!Board.ValidateMove(ref move)) throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                        return move;
                    }
                    // Black Castle Queenside
                    SetFrom(ref move, Square.e8);
                    SetTo(ref move, Square.c8);
                    if (!Board.ValidateMove(ref move)) throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                    return move;
                case "O-O":
                case "0-0":
                    if (Board.CurrentPosition.WhiteMove)
                    {
                        // White Castle Kingside
                        SetFrom(ref move, Square.e1);
                        SetTo(ref move, Square.g1);
                        if (!Board.ValidateMove(ref move)) throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                        return move;
                    }
                    // Black Castle Kingside
                    SetFrom(ref move, Square.e8);
                    SetTo(ref move, Square.g8);
                    if (!Board.ValidateMove(ref move)) throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                    return move;
            }
            var length = standardAlgebraicNoCheck.Length;
            var fromFile = -1;
            var fromRank = -1;
            var promotedPiece = Piece.None;
            int piece;
            int toSquare;
            if (char.IsLower(standardAlgebraicNoCheck, 0))
            {
                // Pawn Move
                piece = Board.CurrentPosition.WhiteMove ? Piece.WhitePawn : Piece.BlackPawn;
                fromFile = Board.Files[Board.GetSquare($"{standardAlgebraicNoCheck[0]}1")];
                switch (length)
                {
                    case 2:
                        // Pawn Move
                        toSquare = Board.GetSquare(standardAlgebraicNoCheck);
                        break;
                    case 4 when standardAlgebraicNoCheck[1] == 'x':
                        // Pawn Capture
                        toSquare = Board.GetSquare(standardAlgebraicNoCheck.Substring(2, 2));
                        break;
                    case 4 when standardAlgebraicNoCheck[2] == '=':
                        // Pawn promotion.  Set case of promoted piece character based on side to move.
                        toSquare = Board.GetSquare(standardAlgebraicNoCheck.Substring(0, 2));
                        promotedPiece = Piece.ParseChar(Board.CurrentPosition.WhiteMove
                            ? char.ToUpper(standardAlgebraicNoCheck[length - 1])
                            : char.ToLower(standardAlgebraicNoCheck[length - 1]));
                        break;
                    case 6:
                        // Pawn promotion with capture.  Set case of promoted piece character based on side to move.
                        toSquare = Board.GetSquare(standardAlgebraicNoCheck.Substring(2, 2));
                        promotedPiece = Piece.ParseChar(Board.CurrentPosition.WhiteMove
                            ? char.ToUpper(standardAlgebraicNoCheck[length - 1])
                            : char.ToLower(standardAlgebraicNoCheck[length - 1]));
                        break;
                    default:
                        throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                }
            }
            else
            {
                // Piece Move
                piece = Piece.ParseChar(Board.CurrentPosition.WhiteMove
                    ? char.ToUpper(standardAlgebraicNoCheck[0])
                    : char.ToLower(standardAlgebraicNoCheck[0]));
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (standardAlgebraicNoCheck[1] == 'x')
                {
                    // Piece Capture
                    var square = standardAlgebraicNoCheck.Substring(2, 2);
                    toSquare = Board.GetSquare(square);
                }
                else if (standardAlgebraicNoCheck[2] == 'x')
                {
                    // Piece Capture with Disambiguation
                    var square = standardAlgebraicNoCheck.Substring(3, 2);
                    toSquare = Board.GetSquare(square);
                    if (char.IsLetter(standardAlgebraicNoCheck[1])) fromFile = Board.Files[Board.GetSquare($"{standardAlgebraicNoCheck[1]}1")]; // Piece disambiguated by file.
                    else fromRank = Board.WhiteRanks[Board.GetSquare($"a{standardAlgebraicNoCheck[1]}")]; // Piece disambiguated by rank.
                }
                else if (length == 3)
                {
                    // Piece Move
                    var square = standardAlgebraicNoCheck.Substring(1, 2);
                    toSquare = Board.GetSquare(square);
                }
                else if (length == 4)
                {
                    // Piece Move with Disambiguation
                    var square = standardAlgebraicNoCheck.Substring(2, 2);
                    toSquare = Board.GetSquare(square);
                    if (char.IsLetter(standardAlgebraicNoCheck[1])) fromFile = Board.Files[Board.GetSquare($"{standardAlgebraicNoCheck[1]}1")]; // Piece disambiguated by file.
                    else fromRank = Board.WhiteRanks[Board.GetSquare($"a{standardAlgebraicNoCheck[1]}")]; // Piece disambiguated by rank.
                }
                else throw new Exception($"{StandardAlgebraic} move not supported.");
            }
            Board.CurrentPosition.GenerateMoves();
            for (var moveIndex = 0; moveIndex < Board.CurrentPosition.MoveIndex; moveIndex++)
            {
                move = Board.CurrentPosition.Moves[moveIndex];
                if (!Board.IsMoveLegal(ref move)) continue; // Skip illegal move.
                var movePiece = Board.CurrentPosition.GetPiece(From(move));
                if (movePiece != piece) continue; // Wrong Piece
                var moveToSquare = To(move);
                if (moveToSquare != toSquare) continue; // Wrong Square
                var movePromotedPiece = PromotedPiece(move);
                if (movePromotedPiece != promotedPiece) continue; // Wrong Promoted Piece
                if (fromFile >= 0)
                {
                    // Piece disambiguated by file.
                    var moveFromFile = Board.Files[From(move)];
                    if (moveFromFile != fromFile) continue; // Wrong File
                }
                if (fromRank >= 0)
                {
                    // Piece disambiguated by rank.
                    // Use white ranks regardless of side to move.
                    var moveFromRank = Board.WhiteRanks[From(move)];
                    if (moveFromRank != fromRank) continue; // Wrong Rank
                }
                if (!Board.ValidateMove(ref move)) throw new Exception($"Move {StandardAlgebraic} is illegal in position {Board.CurrentPosition.ToFen()}.");
                return move;
            }
            throw new Exception($"Failed to parse {StandardAlgebraic} standard algebraic notation move.");
        }


        public static bool IsValid(ulong Move)
        {
            Debug.Assert(CaptureVictim(Move) >= Piece.None);
            Debug.Assert(CaptureVictim(Move) < Piece.BlackKing);
            Debug.Assert(CaptureVictim(Move) != Piece.WhiteKing);
            Debug.Assert(CaptureVictim(Move) != Piece.BlackKing);
            Debug.Assert(CaptureAttacker(Move) >= Piece.None);
            Debug.Assert(CaptureAttacker(Move) <= Piece.BlackKing);
            Debug.Assert(PromotedPiece(Move) >= Piece.None);
            Debug.Assert(PromotedPiece(Move) < Piece.BlackKing);
            Debug.Assert(PromotedPiece(Move) != Piece.WhitePawn);
            Debug.Assert(PromotedPiece(Move) != Piece.BlackPawn);
            Debug.Assert(PromotedPiece(Move) != Piece.WhiteKing);
            Debug.Assert(PromotedPiece(Move) != Piece.BlackKing);
            Debug.Assert(Killer(Move) >= 0);
            Debug.Assert(Killer(Move) <= 2);
            Debug.Assert(From(Move) >= Square.a8);
            Debug.Assert(From(Move) <= Square.Illegal);
            Debug.Assert(To(Move) >= Square.a8);
            Debug.Assert(To(Move) <= Square.Illegal);
            return true;
        }


        public static string ToLongAlgebraic(ulong Move)
        {
            if (Move == Null) return "Null";
            var fromSquare = From(Move);
            var toSquare = To(Move);
            var promotedPiece = PromotedPiece(Move);
            return $"{Board.SquareLocations[fromSquare]}{Board.SquareLocations[toSquare]}{(promotedPiece == Piece.None ? string.Empty : Piece.GetChar(promotedPiece).ToString().ToLower())}";
        }


        public static string ToString(ulong Move)
        {
            return $"{ToLongAlgebraic(Move)} (B = {IsBest(Move)}, CapV = {Piece.GetChar(CaptureVictim(Move))}, CapA = {Piece.GetChar(CaptureAttacker(Move))}, Promo = {Piece.GetChar(PromotedPiece(Move))}, Kil = {Killer(Move)}, " +
                   $"! = {Played(Move)},  O = {IsCastling(Move)}, K = {IsKingMove(Move)}, E = {IsEnPassantCapture(Move)}, 2 = {IsDoublePawnMove(Move)}, P = {IsPawnMove(Move)}, C = {IsCheck(Move)}, Q = {IsQuiet(Move)} " +
                $"From = {From(Move)}, To = {To(Move)}";
        }
    }
}