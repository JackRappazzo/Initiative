import React from 'react';
import './Pagination.css';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  disabled?: boolean;
}

const MAX_PAGE_BUTTONS = 7;

const Pagination: React.FC<PaginationProps> = ({ currentPage, totalPages, onPageChange, disabled = false }) => {
  if (totalPages <= 1) return null;

  const pageNumbers = buildPageNumbers(currentPage, totalPages, MAX_PAGE_BUTTONS);

  return (
    <div className="pagination">
      <button
        className="pagination-btn"
        onClick={() => onPageChange(currentPage - 1)}
        disabled={disabled || currentPage === 1}
      >
        &lsaquo; Prev
      </button>
      {pageNumbers.map((entry, i) =>
        entry === null ? (
          <span key={`ellipsis-${i}`} className="pagination-ellipsis">&hellip;</span>
        ) : (
          <button
            key={entry}
            className={`pagination-btn ${entry === currentPage ? 'pagination-btn--active' : ''}`}
            onClick={() => onPageChange(entry)}
            disabled={disabled}
          >
            {entry}
          </button>
        )
      )}
      <button
        className="pagination-btn"
        onClick={() => onPageChange(currentPage + 1)}
        disabled={disabled || currentPage === totalPages}
      >
        Next &rsaquo;
      </button>
    </div>
  );
};

function buildPageNumbers(current: number, total: number, maxButtons: number): (number | null)[] {
  if (total <= maxButtons) {
    return Array.from({ length: total }, (_, i) => i + 1);
  }
  const pages: (number | null)[] = [];
  const half = Math.floor((maxButtons - 2) / 2);
  let rangeStart = Math.max(2, current - half);
  let rangeEnd = Math.min(total - 1, current + half);

  if (current - half < 2) rangeEnd = Math.min(total - 1, maxButtons - 2);
  if (current + half > total - 1) rangeStart = Math.max(2, total - (maxButtons - 3));

  pages.push(1);
  if (rangeStart > 2) pages.push(null);
  for (let p = rangeStart; p <= rangeEnd; p++) pages.push(p);
  if (rangeEnd < total - 1) pages.push(null);
  pages.push(total);
  return pages;
}

export default Pagination;
